using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public static class NanoSystemValidator
{
    public static bool ValidateSystemIntersectionsClassic(IList<Particle> particles)
    {
        foreach (var p1 in particles)
        {
            foreach (var p2 in particles)
            {
                if(p1 == p2) continue;

                if (p1.IsIntersect(p2))
                {
                    File.WriteAllLines("/Users/danilalatyrev/Desktop/Projects/SaxsSpot/SaxsSpot.NanoSystemGeneration/SaxsSpot.NanoSystemGeneration.Tests/log", [p1.ToString(), p2.ToString()]);
                    return false;
                }
            }
        }
        
        return true;
    }

    
    public static bool ValidateSystemIntersections(IList<Particle> particles, GenerationZone zone)
    {
        var randomVectors = GenerateRandomVectors(100000, zone);

        foreach (var randomVector in randomVectors)
        {
            if (particles
                    .AsParallel()
                    .Count(x => IntersectionService.IsPointInsideParticle(randomVector, x)) > 1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool ValidateGenerationZone(GenerationZone zone, IList<Particle> particles)
    {
        var extremeParticles = new[] 
            {particles.MaxBy(x => x.X), particles.MaxBy(x => x.Y), particles.MaxBy(x => x.Z)};

        foreach (var extremeParticle in extremeParticles)
        {
            if (!IntersectionService.IsParticleInsideZone(extremeParticle, zone))
            {
                return false;
            }
        }

        return true;
    }
    
    private static List<Vector<float>> GenerateRandomVectors(int count, GenerationZone zone)
    {
        var random = new Random();
        var list = new List<Vector<float>>();

        for (int i = 0; i < count; i++)
        {
            Vector<float> vector;

            if (zone.GenerationZoneForm == GenerationZoneForm.Cube)
            {
                vector = Vector<float>.Build.DenseOfArray(new float[]
                {
                    (random.NextSingle() * 2 - 1) * zone.GlobalSize,
                    (random.NextSingle() * 2 - 1) * zone.GlobalSize,
                    (random.NextSingle() * 2 - 1) * zone.GlobalSize
                });
            }
            else
            {
                Vector<float> point;
                do
                {
                    point = Vector<float>.Build.DenseOfArray(new float[]
                    {
                        (random.NextSingle() * 2 - 1),
                        (random.NextSingle() * 2 - 1),
                        (random.NextSingle() * 2 - 1)
                    });
                } while (point.L2Norm() > 1);

                vector = point * zone.GlobalSize;
            }

            list.Add(vector);
        }

        return list;
    }
}