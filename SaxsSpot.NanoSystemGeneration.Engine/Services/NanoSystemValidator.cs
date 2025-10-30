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
                    File.WriteAllLines("C:\\Projects\\SaxsSpot.NanoSystemGeneration\\SaxsSpot.NanoSystemGeneration.Tests\\intersections", [p1.ToString(), p2.ToString()]);
                    return false;
                }
            }
        }
        
        return true;
    }

    
    public static bool ValidateSystemIntersections(IList<Particle> particles, GenerationZone zone)
    {
        var randomVectors = GenerateRandomVectors(100000, zone);
        var intersects = new List<(List<Particle>, Vector<float>)>();
        foreach (var randomVector in randomVectors)
        {
            var intersected = particles
                .AsParallel()
                .Where(x => IntersectionService.IsPointInsideParticle(randomVector, x))
                .ToList();
            if (intersected.Count > 1)
            {
                intersects.Add((intersected, randomVector));
            }
        }

        if (intersects.Any(intersected => intersected.Item1.Count > 1))
        {
            foreach (var intersected in intersects)
            {
                File.AppendAllLines("C:\\Projects\\SaxsSpot.NanoSystemGeneration\\SaxsSpot.NanoSystemGeneration.Tests\\intersections", 
                    [string.Join("\n", intersected.Item1.Select(x => x.ToString())), intersected.Item2.ToString()]);
            }
            return false;
        }

        return true;
    }
    
    public static List<(List<Particle>, Vector<float>)> GetSystemIntersections(IList<Particle> particles, GenerationZone zone)
    {
        var randomVectors = GenerateRandomVectors(100000, zone);
        var intersects = new List<(List<Particle>, Vector<float>)>();
        foreach (var randomVector in randomVectors)
        {
            var intersected = particles
                .AsParallel()
                .Where(x => IntersectionService.IsPointInsideParticle(randomVector, x))
                .ToList();
            if (intersected.Count > 1)
            {
                intersects.Add((intersected, randomVector));
            }
        }

        return intersects;
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
                    (random.NextSingle() * 2 - 1) * (float)zone.GlobalSize,
                    (random.NextSingle() * 2 - 1) * (float)zone.GlobalSize,
                    (random.NextSingle() * 2 - 1) * (float)zone.GlobalSize
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

                vector = point * (float)zone.GlobalSize;
            }

            list.Add(vector);
        }

        return list;
    }
}