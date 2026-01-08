using System.Collections.Concurrent;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public static class NanoSystemValidator
{
    public static bool ValidateSystemIntersectionsClassicSphere(IList<Sphere> particles)
    {
        foreach (var p1 in particles)
        {
            foreach (var p2 in particles)
            {
                if(p1 == p2) continue;

                if (IntersectionService.IsSphereIntersect(p1 ,p2))
                {
                    File.WriteAllLines("C:\\Projects\\SaxsSpot\\SaxsSpot.NanoSystemGeneration\\SaxsSpot.NanoSystemGeneration.Tests\\intersections", [p1.ToString(), p2.ToString()]);
                    return false;
                }
            }
        }
        
        return true;
    }
    
    public static bool ValidateSystemIntersectionsClassicParallelepiped(IList<Parallelepiped> particles)
    {
        foreach (var p1 in particles)
        {
            foreach (var p2 in particles)
            {
                if(p1 == p2) continue;

                if (IntersectionService.IsIntersect(p1 ,p2))
                {
                    File.WriteAllLines("C:\\Projects\\SaxsSpot.NanoSystemGeneration\\SaxsSpot.NanoSystemGeneration.Tests\\intersections", [p1.ToString(), p2.ToString()]);
                    return false;
                }
            }
        }
        
        return true;
    }

    
    public static (bool, int) ValidateSystemIntersections(IList<Particle> particles, GenerationZone zone, int pointCount, IProgress<float> progress = null)
    {
        var randomVectors = RandomVectorGenerator.GenerateRandomVectors(pointCount, zone);
        var intersects = new ConcurrentBag<(List<Particle>, Vector<float>)>();

        var c = 0;
        Parallel.ForEach(randomVectors, randomVector =>
        {
            var intersected = particles
                .AsParallel()
                .Where(x => IntersectionService.IsPointInsideParticle(randomVector, x))
                .ToList();

            if (intersected.Count > 1)
            {
                intersects.Add((intersected, randomVector));
            }

            c++;
            progress.Report(c / (float)pointCount);
        });
    
        //TODO distinct
        var distinctIntersects = 
            intersects.DistinctBy(x => 
                string.Join(',', x.Item1.OrderBy(x => x.GetVolume())
                    .Select(x => x.ToString())))
                .ToList();
        
        if (distinctIntersects.Any(intersected => intersected.Item1.Count > 1))
        {
            foreach (var intersected in distinctIntersects)
            {
                File.AppendAllLines("/Users/danilalatyrev/Desktop/Projects/SaxsSpot/SaxsSpot.NanoSystemGeneration/SaxsSpot.NanoSystemGeneration.Tests/intersections", 
                    [string.Join("\n", intersected.Item1.Select(x => x.ToString())), intersected.Item2.ToString()]);
            }
            return (false, distinctIntersects.Count);
        }

        return (true, 0);
    }
    
    public static List<(List<Particle>, Vector<float>)> GetSystemIntersections(IList<Particle> particles, GenerationZone zone)
    {
        var randomVectors = RandomVectorGenerator.GenerateRandomVectors(10000, zone);
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
}