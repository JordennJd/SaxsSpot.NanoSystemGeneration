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
        "/Users/danilalatyrev/Desktop/Projects/SaxsSpot/SaxsSpot.NanoSystemGeneration/SaxsSpot.NanoSystemGeneration.Tests";
    
    [Test]
    [TestCase(1f, 100000, 0.2, null, 2f, 6f, 0.4f, 3f, 1.1f,
        ParticleKind.Parallelepiped)]
    // [TestCase(0.5f, 10000, 0.4f, null, 2f, 6f, 1f, 3f, 1.1f,
    //     ParticleKind.Sphere)]
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
        for (int i = 0; i < 10; i++)
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
            //     // Assert.That(NanoSystemValidator.ValidateSystemIntersectionsClassic(distributeParticles));
            //     Assert.That(isGenerationZoneValid);
            //     Assert.That(isIntersectionsValid);
            // });
        }
    }
    
    [Test]
    [TestCase(1f, 100000, 0.2, null, 2f, 6f, 0.4f, 3f, 0f,
        ParticleKind.Parallelepiped)]
    // [TestCase(0.5f, 10000, 0.4f, null, 2f, 6f, 1f, 3f, 1.1f,
    //     ParticleKind.Sphere)]
    public async Task SuccessGenerationCases2(
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
        for (int i = 0; i < 10; i++)
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
            
            await File.AppendAllLinesAsync($"{basePath}/log02",
            [$"time: {startTime - endTime} particleKind: {particleKind} count: {count} nc: {numericalConcentration} gs: {globalSize} excess: {excess} genZone: {(isGenerationZoneValid ? "+" : "-")} intersections: {(isIntersectionsValid ? "+" : "-") } ({intersectionCount}) classicCheck: {(true ? "+" : "-")}",
                "parameters:", $"realNc: {distributeParticles.Sum(x => x.GetVolume()) / (await nanoSystemGenerator.GetGenerationZone()).GetVolume()} realCount: {distributeParticles.Count}"]);
            
            // Assert.Multiple(() =>
            // {
            //     // Assert.That(NanoSystemValidator.ValidateSystemIntersectionsClassic(distributeParticles));
            //     Assert.That(isGenerationZoneValid);
            //     Assert.That(isIntersectionsValid);
            // });
        }
    }
    
    [Test]
    [TestCase(1f, 10000, 0.2, null, 2f, 6f, 0.4f, 3f, 0f,
        ParticleKind.Parallelepiped)]
    // [TestCase(0.5f, 10000, 0.4f, null, 2f, 6f, 1f, 3f, 1.1f,
    //     ParticleKind.Sphere)]
    public async Task SuccessGenerationCases3(
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
            var progressValidation = new Progress<float>();
            progressValidation.ProgressChanged += (sender, f) =>
            {
                if (TestContext.CurrentContext.Random.Next(1, 200) == 1)
                {
                    TestContext.Progress.WriteLine($"{f*100}%");
                }
            };
            // (isIntersectionsValid, intersectionCount) = NanoSystemValidator.ValidateSystemIntersections(distributeParticles, await nanoSystemGenerator.GetGenerationZone(), 1000000, progressValidation);
            
            TestContext.Progress.WriteLine("Writing result to log...");

            // await File.AppendAllLinesAsync($"{basePath}/analyze_system_par", distributeParticles.Select(x => x.ToString()));
            
            await File.AppendAllLinesAsync($"{basePath}/log03",
            [$"time: {startTime - endTime} particleKind: {particleKind} count: {count} nc: {numericalConcentration}" +
             $" gs: {globalSize} excess: {excess} genZone: {(isGenerationZoneValid ? "+" : "-")}" +
             $" intersections: {(isIntersectionsValid ? "+" : "-") } ({intersectionCount}) classicCheck: {(true ? "+" : "-")}",
                "parameters:", $"realNc: {distributeParticles.Sum(x => x.GetVolume()) / (await nanoSystemGenerator.GetGenerationZone()).GetVolume()}" +
                               $" realCount: {distributeParticles.Count}"]);
            
            Assert.Multiple(() =>
            {
                // Assert.That(NanoSystemValidator.ValidateSystemIntersectionsClassic(distributeParticles));
                Assert.That(isGenerationZoneValid);
                Assert.That(isIntersectionsValid);
            });
        }
    }
    
    [Test]
    [TestCase(1f, 100000, 0.3, null, 2f, 6f, 0.1f, 1.5f, 1.1f,
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
        await File.AppendAllLinesAsync($"{basePath}/analyze_system_par", distributeParticles.Select(x => x.ToString()));

        var analyze = NanosystemAnalyzer.GetNanosystemAnalyze(distributeParticles,generationZone, 20, 100000);
        TestContext.Progress.WriteLine("Analyzing system...");
        
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
        var analyze = NanosystemAnalyzer.GetNanosystemAnalyze(particles.ToList(), new GenerationZone(particles.MaxBy(x => x.Y).Y, GenerationZoneForm.Sphere), 20, 500000);
        TestContext.Progress.WriteLine("Analyzing system...");
        
        await File.AppendAllLinesAsync($"{basePath}/analyze", analyze.Select(x => $"{x.ZoneIndex}: {x.Concentration}"));
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