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
		var volumeStep = globalVolume / zoneCount;

		var currentRadius = 0d;
		var radii = new List<double>(zoneCount);
		for(var i = 0; i < zoneCount; i++)
		{
			currentRadius = Math.Pow((volumeStep + (4f/3f*Math.PI*Math.Pow(currentRadius, 3))) / (4f/3f * Math.PI), 1f / 3f);
			radii.Add(currentRadius);
		}
		
    	var globalZoneRadius = generationZone.GenerationZoneForm == GenerationZoneForm.Cube 
    		? generationZone.GlobalSize / 2 
    		: generationZone.GlobalSize;
	    
	    var pointsByRadii = radii.Select((radius, index) => new
	    {
		    Radius = radius,
		    Points = RandomVectorGenerator.GenerateRandomVectors(
			    vectorCount / radii.Count, 
			    generationZone, 
			    index == 0 ? 0 : radii[index - 1], 
			    radius
		    ),
		    Particles = particles!.Where(particle => 
				    IntersectionService.IsParticleBelongsZone(particle, radius, radius - (index == 0 ? 0 : radii[index - 1])))
			    .ToList()
	    }).ToList();
	    
    	var result = new ConcurrentBag<ZoneConcentrationAnalyze>();
    	var zoneIndex = 0;
	    Parallel.ForEach(pointsByRadii.OrderBy(p => p.Radius), new ParallelOptions()
	    {
	    }, radiusWithPoints =>
	    { 
		    var (radius, points, particlesInBound)
			    = (radiusWithPoints.Radius, radiusWithPoints.Points, radiusWithPoints.Particles);


		    var pointsInParticle = 0f;

		    foreach (var paricle in particlesInBound)
		    {
			    foreach (var point in points)
			    {
				    if (IntersectionService.IsPointInsideParticle(point, paricle))
				    {
					    pointsInParticle++;
				    }
			    }
		    }

		    result.Add(new ZoneConcentrationAnalyze(zoneIndex, pointsInParticle / points.Count()));
		    zoneIndex++;
	    });
    	
    	return result.OrderBy(x => x.ZoneIndex).ToList();
    }
}