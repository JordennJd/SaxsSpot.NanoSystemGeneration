using NUnit.Framework.Internal;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Services;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace SaxsSpot.NanoSystemGeneration.Tests;

public class ParticleGenerationTests
{
    private readonly static string basePath =
        "C:\\Projects\\SaxsSpot\\SaxsSpot.NanoSystemGeneration\\SaxsSpot.NanoSystemGeneration.Tests\\";
    
    [Test]
    [TestCase(1f, 100000, 0.5, null, 2f, 6f, 0.4f, 3f, 1.1f,
        ParticleKind.Parallelepiped)]
    public async Task SuccessGenerationCases(
        float epsilon,
        int count,
        double? numericalConcentration,
        float? globalSize,
        float minSize,
        float maxSize,
        float theta,
        float k,
        float excess,
        ParticleKind particleKind
        )
    {
        for (int i = 0; i < 1; i++)
        { 
            GC.Collect();
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
            
            var isGenerationZoneValid = true;
            var (isIntersectionsValid, intersectionCount) = (true, 0);
            // (isIntersectionsValid, intersectionCount) = NanoSystemValidator.ValidateSystemIntersections(distributeParticles, await nanoSystemGenerator.GetGenerationZone());
            
            TestContext.Progress.WriteLine("Writing result to log...");

            // await File.AppendAllLinesAsync($"{basePath}/analyze_system_par", distributeParticles.Select(x => x.ToString()));
            
            await File.AppendAllLinesAsync($"{basePath}/log01",
            [$"time: {startTime - endTime} particleKind: {particleKind} count: {count} nc: {numericalConcentration} gs: {globalSize} excess: {excess} genZone: {(isGenerationZoneValid ? "+" : "-")} intersections: {(isIntersectionsValid ? "+" : "-") } ({intersectionCount}) classicCheck: {(true ? "+" : "-")}",
                "parameters:", $"realNc: {distributeParticles.Sum(x => x.GetVolume()) / (await nanoSystemGenerator.GetGenerationZone()).GetVolume()} realCount: {distributeParticles.Count}"]);
            
            // Assert.Multiple(() =>
            // {
                Assert.That(NanoSystemValidator
                    .ValidateSystemIntersectionsClassicParallelepiped(distributeParticles.Select(x => (Parallelepiped)x).ToList()));
            //     Assert.That(isGenerationZoneValid);
            //     Assert.That(isIntersectionsValid);
            // });
        }
    }
    
    [Test]
    [TestCase(1f, 10000, 0.2, null, 2f, 6f, 0.1f, 1.5f, 0,
        ParticleKind.Parallelepiped)]
    public async Task GenerateAndAnalyze(
        float epsilon,
        int count,
        double? numericalConcentration,
        float? globalSize,
        float minSize,
        float maxSize,
        float rate,
        float k,
        float excess,
        ParticleKind particleKind
        )
    {
        ParticleGenerationParameters? generationParameters = particleKind switch
        {
            ParticleKind.Parallelepiped => new ParallelepipedGenerationParameters(epsilon, count,
                numericalConcentration, globalSize, minSize, maxSize, rate, k, excess),
            ParticleKind.Sphere => new SphereGenerationParameters(count, numericalConcentration, globalSize, minSize,
                maxSize, rate, k, excess),
            _ => throw new ArgumentOutOfRangeException(nameof(particleKind), particleKind, null)
        };

        var nanoSystemGenerator = new NanoSystemGenerator(generationParameters);
        TestContext.Progress.WriteLine("Generating particles...");
        var startTime = DateTime.Now;
        var system = await nanoSystemGenerator.GenerateSystem();
        var progress = new Progress<float>();
        
        progress.ProgressChanged += (sender, f) =>
        {
            if (TestContext.CurrentContext.Random.Next(1, 200) == 1)
            {
                TestContext.Progress.WriteLine($"{f}%");
            }
        };
        
        TestContext.Progress.WriteLine("Distributing particles...");
        var distributeParticles = await nanoSystemGenerator.DistributeParticles(progress, CancellationToken.None);
        var generationZone = await nanoSystemGenerator.GetGenerationZone();
        await File.AppendAllLinesAsync($"{basePath}/analyze_system_par", distributeParticles.Select(x => x.ToString()));

        var analyze = NanosystemAnalyzer.GetNanosystemAnalyze(distributeParticles,generationZone, 2, 1000000);
        TestContext.Progress.WriteLine("Analyzing system...");
        await File.AppendAllLinesAsync($"{basePath}/log01",
        [$"particleKind: {particleKind} count: {count} nc: {numericalConcentration} gs: {globalSize} excess: {excess}",
            "parameters:", $"realNc: {distributeParticles.Sum(x => x.GetVolume()) / (await nanoSystemGenerator.GetGenerationZone()).GetVolume()} realCount: {distributeParticles.Count}"]);

        await File.AppendAllLinesAsync($"{basePath}/analyze", analyze.Select(x => $"{x.ZoneIndex}: {x.Concentration}"));
    }
    
    [Test]
    public async Task AnalyzeSphereSystem()
    {
        var particles = File
            .ReadLines(
                $"{basePath}/analyze_system")
            .Select(x => Sphere.FromOldString(x));
        var analyze = NanosystemAnalyzer.GetNanosystemAnalyze(particles.ToList(), new GenerationZone(particles.MaxBy(x => x.X).X * 2, GenerationZoneForm.Cube), 1, 1000000);
        TestContext.Progress.WriteLine("Analyzing system...");
        
        await File.AppendAllLinesAsync($"{basePath}/analyze", analyze.Select(x => $"{x.ZoneIndex}: {x.Concentration}"));
    }
    
    [Test]
    public async Task AnalyzeParallelepipedSystem()
    {
        var particles = File
            .ReadLines(
                $"{basePath}/analyze_system_par")
            .Select(x => Parallelepiped.FromString(x));
        TestContext.Progress.WriteLine("Analyzing system...");
        var analyze = NanosystemAnalyzer.GetNanosystemAnalyze(particles.ToList(), new GenerationZone(particles.MaxBy(x => x.Y).Y, GenerationZoneForm.Sphere), 20, 10000000);
        
        await File.AppendAllLinesAsync($"{basePath}/analyze", analyze.Select(x => $"{x.ZoneIndex}: {x.Concentration}"));
    }

    [Test]
    public async Task ValidateSystemFromFile()
    {
        var system = File
            .ReadAllLines(basePath+"toValidate")
            .Select(Sphere.FromOldString);

        var result = NanoSystemValidator.ValidateSystemIntersectionsClassicSphere(system.ToList());
        
        Assert.That(result);
    }
}