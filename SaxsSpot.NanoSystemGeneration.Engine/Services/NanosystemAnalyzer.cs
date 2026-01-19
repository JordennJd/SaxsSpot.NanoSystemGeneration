using System.Collections.Concurrent;
using MathNet.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public static class NanosystemAnalyzer
{
	public static ICollection<ZoneConcentrationAnalyze> 
		GetNanosystemAnalyze<T>
		(ICollection<T> particles, GenerationZone generationZone, int zoneCount, int vectorCount) where T : Particle
	{
		// V = 4/3*pi*r^3
		// r = (V/(4/3*pi))^1/3
		var globalVolume = generationZone.GetVolume();
		var volumeStep = globalVolume / (zoneCount-1);

		var currentRadius = 0d;
		var radii = new List<double>(zoneCount);
		for(var i = 0; i < (zoneCount-1); i++)
		{
			currentRadius = Math.Pow((volumeStep + (4f/3f*Math.PI*Math.Pow(currentRadius, 3))) / (4f/3f * Math.PI), 1f / 3f);
			radii.Add(currentRadius);
		}
		
		var bounds = radii.Select((radius, index) => new
	    {
		    ZoneIndex = index,
		    InnerRadius = index == 0 ? 0 : radii[index - 1],
		    OuterRadius = radius,
	    }).ToList();
		
		particles
			.AsParallel()
			.OfType<Parallelepiped>()
			.ForAll(ParallelepipedManipulator.PrepareParallelepiped);

    	var result = new ConcurrentBag<ZoneConcentrationAnalyze>();
	    Parallel.ForEach(bounds.OrderBy(p => p.ZoneIndex), new ParallelOptions()
	    {
		    MaxDegreeOfParallelism = Environment.ProcessorCount
	    }, bound =>
	    {
		    var points = RandomVectorGenerator.GenerateRandomVectors(
			    vectorCount / zoneCount,
			    generationZone
		    ).Where(x => x.L2Norm() >= bound.InnerRadius && x.L2Norm() <= bound.OuterRadius);
		    
		    var pointsInParticle = 0f;
		    var inBound = particles.Where(x =>
			    IntersectionService.IsParticleBelongsZone(x, bound.OuterRadius, bound.OuterRadius - bound.InnerRadius));
		    foreach (var paricle in inBound)
		    {
			    foreach (var point in points)
			    {
				    if (IntersectionService.IsPointInsideParticle(point, paricle))
				    {
					    pointsInParticle++;
				    }
			    }
		    }

		    result.Add(new ZoneConcentrationAnalyze(bound.ZoneIndex, pointsInParticle / points.Count()));
	    });
    	
    	return result.OrderBy(x => x.ZoneIndex).ToList();
    }
}