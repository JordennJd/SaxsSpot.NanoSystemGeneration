using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Services;

namespace SaxsSpot.NanoSystemGeneration.Tests;

public class ParticleGenerationTests
{
    [Test]
    [TestCase(1f, 10000, 0.2f, null, 2f, 6f, 1f, 6f, 1.1f,
        ParticleKind.Parallelepiped)]
    // [TestCase(0.5f, 10000, 0.4f, null, 2f, 6f, 1f, 3f, 1.1f,
    //     ParticleKind.Sphere)]
    public async Task SuccessGenerationCases(
        float epsilon,
        int count,
        float? numericalConcentration,
        float? globalSize,
        float minSize,
        float maxSize,
        float theta,
        float k,
        float excess,
        ParticleKind particleKind
        )
    {
        ParticleGenerationParameters? generationParameters = particleKind switch
        {
            ParticleKind.Parallelepiped => new ParallelepipedGenerationParameters(epsilon, count,
                numericalConcentration, globalSize, minSize, maxSize, theta, k, excess),
            ParticleKind.Sphere => new SphereGenerationParameters(count, numericalConcentration, globalSize, minSize,
                maxSize, theta, k, excess),
            _ => throw new ArgumentOutOfRangeException(nameof(particleKind), particleKind, null)
        };

        var nanoSystemGenerator = new NanoSystemGenerator(generationParameters);
        TestContext.Progress.WriteLine("Generating particles...");
        var startTime = DateTime.Now;
        var system = await nanoSystemGenerator.GenerateSystem();
        var generationZone = await nanoSystemGenerator.GetGenerationZone();
        var progress = new Progress<float>();

        var progressLog = "C:\\Projects\\SaxsSpot.NanoSystemGeneration\\SaxsSpot.NanoSystemGeneration.Tests\\Progress";
        File.Delete(progressLog);
        File.Create(progressLog);
        progress.ProgressChanged += (sender, f) =>
        {
            if (TestContext.CurrentContext.Random.Next(1, 200) == 1)
            {
                TestContext.Progress.WriteLine($"{f}%");
            }
        };
        
        TestContext.Progress.WriteLine("Distributing particles...");
        var distributeParticles = await nanoSystemGenerator.DistributeParticles(progress, CancellationToken.None);
        var endTime = DateTime.Now;

        TestContext.Progress.WriteLine("Validating system...");
        
        var isGenerationZoneValid =
            NanoSystemValidator.ValidateGenerationZone(await nanoSystemGenerator.GetGenerationZone(),
                distributeParticles);
        var isIntersectionsValid =
            NanoSystemValidator.ValidateSystemIntersections(distributeParticles,
                await nanoSystemGenerator.GetGenerationZone());
        
        TestContext.Progress.WriteLine("Writing result to log...");

        await File.AppendAllLinesAsync("C:\\Projects\\SaxsSpot.NanoSystemGeneration\\SaxsSpot.NanoSystemGeneration.Tests\\log",
        [$"time: {startTime - endTime} particleKind: {particleKind} count: {count} nc: {numericalConcentration} gs: {globalSize} excess: {excess} genZone: {(isGenerationZoneValid ? "+" : "-")} intersections: {(isIntersectionsValid ? "+" : "-")}",
            "parameters:", $"realNc: {distributeParticles.Sum(x => x.GetVolume()) / (await nanoSystemGenerator.GetGenerationZone()).GetVolume()} realCount: {distributeParticles.Count}"]);
        
        Assert.Multiple(() =>
        {
            // Assert.That(NanoSystemValidator.ValidateSystemIntersectionsClassic(distributeParticles));
            Assert.That(isGenerationZoneValid);
            Assert.That(isIntersectionsValid);
        });
    }
    
    // [Test]
    // [TestCase(1f, -1, 0.2f, null, 1f, 3f, 1f, 6f, 0,
    //     ParticleKind.Parallelepiped)]
    // [TestCase(1f, 1, 0.2f, null, 1f, 3f, 1f, 6f, 0,
    //     ParticleKind.Sphere)]
    // [TestCase(1f, 1000, -0.34f, null, 1f, 3f, 1f, 6f, 0,
    //     ParticleKind.Sphere)]
    // [TestCase(1f, 1000, 0.4f, null, 1f, 3f, 1f, 6f, 0,
    //     ParticleKind.Sphere)]
    // [TestCase(0.5f, 1000, 0.39f, null, 1f, 3f, 1f, 6f, 0,
    //     ParticleKind.Parallelepiped)]
    // public void ErrorGenerationCases(
    //     float epsilon,
    //     int count,
    //     float? numericalConcentration,
    //     float? globalSize,
    //     float minSize,
    //     float maxSize,
    //     float theta,
    //     float k,
    //     float excess,
    //     ParticleKind particleKind
    // )
    // {
    //     ParticleGenerationParameters? generationParameters = particleKind switch
    //     {
    //         ParticleKind.Parallelepiped => new ParallelepipedGenerationParameters(epsilon, count,
    //             numericalConcentration, globalSize, minSize, maxSize, theta, k, excess),
    //         ParticleKind.Sphere => new SphereGenerationParameters(count, numericalConcentration, globalSize, minSize,
    //             maxSize, theta, k, excess),
    //         _ => throw new ArgumentOutOfRangeException(nameof(particleKind), particleKind, null)
    //     };
    //
    //     var ex = Assert.Catch(() => _nanoSystemGenerationService.Generate(generationParameters, new Progress<float>(), CancellationToken.None).Wait());
    //     
    //     Assert.That(ex, Is.Not.Null);
    // }
}