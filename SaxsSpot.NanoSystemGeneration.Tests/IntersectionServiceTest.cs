using System.Globalization;
using System.Numerics;
using Extreme.Mathematics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Services;

namespace SaxsSpot.NanoSystemGeneration.Tests;

public class IntersectionServiceTest
{
    [Test]
    public async Task ShouldNotIntersect()
    {
        var p1 = Parallelepiped.FromString
            ("5.5815296 1 -21.127586 35.804977 -25.133026 0.4084141 -0.16429679 1.493728");
        
        var p2 = Parallelepiped.FromString
            ("4.9370775 1 -20.748661 29.20415 -25.75789 -0.19859473 0.5414058 1.2596422");

        await Assert.ThatAsync(() => Task.FromResult(IntersectionService.IsIntersect(p1, p2)), Is.False);
    }
    
    public async Task TestIntersectionServiceIsPointInsideParticle()
    {
        var p2 = Parallelepiped.FromString
            ($"4.9370775 1 0 0 0 0 0 0");

        var points = ParallelepipedCoverer.FillGenerationZoneWithPoints(new GenerationZone(4.9370775, GenerationZoneForm.Cube), 100000);
        var c = 0;
        foreach (var point in points)
        {
            if (IntersectionService.IsPointInParallelepiped(point, p2))
            {
                c++;
            }
        }
        
        Console.WriteLine(c);
    }
    
    [Test]
    [TestCase(1f, 10000, 0.3, null, 2f, 6f, 2f, 50f, 0,
        ParticleKind.Sphere)]
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
        
        var system = await nanoSystemGenerator.GenerateSystem();
        var distributeParticles = await nanoSystemGenerator.DistributeParticles(new Progress<float>(), CancellationToken.None);
        var generationZone = await nanoSystemGenerator.GetGenerationZone();

        var points = RandomVectorGenerator.GenerateRandomVectors(1000000, generationZone);

        var pointsInsideInnerSphere = points
            .Where(x => x.L2Norm() >= 0 && x.L2Norm() <= (generationZone.GenerationZoneForm
                                                          == GenerationZoneForm.Cube
                ? generationZone.GlobalSize / 2
                : generationZone.GlobalSize));

        var pointsOutsideInnerSphere = points.Except(pointsInsideInnerSphere);
        
        distributeParticles
			.AsParallel()
			.OfType<Parallelepiped>()
			.ForAll(ParallelepipedManipulator.PrepareParallelepiped);

        
        var c1 = 0;
        Parallel.ForEach(distributeParticles, particle =>
        {
            foreach (var point in pointsOutsideInnerSphere)
            {
                if (IntersectionService.IsPointInsideParticle(point, particle))
                {
                    Interlocked.Increment(ref c1);
                }
            }
        });
        
        var c2 = 0;
        Parallel.ForEach(distributeParticles, particle =>
        {
            foreach (var point in pointsInsideInnerSphere)
            {
                if (IntersectionService.IsPointInsideParticle(point, particle))
                {
                    Interlocked.Increment(ref c2);
                }
            }
        });
        
        var c3 = 0;
        Parallel.ForEach(distributeParticles, particle =>
        {
            foreach (var point in points)
            {
                if (IntersectionService.IsPointInsideParticle(point, particle))
                {
                    Interlocked.Increment(ref c3);
                }
            }
        });

        TestContext.Progress.WriteLine((double)c1 / (double)pointsOutsideInnerSphere.Count());
        TestContext.Progress.WriteLine((double)c2 / (double)pointsInsideInnerSphere.Count());
        TestContext.Progress.WriteLine((double)c3 / (double)points.Count());

        TestContext.Progress.WriteLine(distributeParticles.Sum(x => x.GetVolume()) / (double)generationZone.GetVolume());

    }
}