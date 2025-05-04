using NUnit.Framework.Internal;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Services;

namespace SaxsSpot.NanoSystemGeneration.Tests;

public class ParticleGenerationTests
{
    [Test]
    [TestCase(1f, 100000, 0.4f, null, 1f, 3f, 1f, 6f, 0,
        ParticleKind.Parallelepiped)]
    [TestCase(0.5f, 100000, 0.4f, null, 1f, 3f, 1f, 3f, 1.1f,
        ParticleKind.Sphere)]

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
        
        var system = await nanoSystemGenerator.GenerateSystem();
        var progress = new Progress<float>();
        progress.ProgressChanged += (sender, f) =>
        {
            Console.Write($"\r {f}%");
        };
        
        TestContext.Progress.WriteLine("Distributing particles...");
        var distributeParticles = await nanoSystemGenerator.DistributeParticles(progress, CancellationToken.None);

        TestContext.Progress.WriteLine("Validating system...");
        await Assert.MultipleAsync(async () =>
        {
            // Assert.That(NanoSystemValidator.ValidateSystemIntersectionsClassic(distributeParticles));
            Assert.That(NanoSystemValidator.ValidateGenerationZone(await nanoSystemGenerator.GetGenerationZone(), distributeParticles));
            Assert.That(NanoSystemValidator.ValidateSystemIntersections(distributeParticles, await nanoSystemGenerator.GetGenerationZone()), Is.True);
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