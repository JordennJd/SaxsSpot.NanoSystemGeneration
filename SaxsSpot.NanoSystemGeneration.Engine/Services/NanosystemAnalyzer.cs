using System.Collections.Concurrent;
using MathNet.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using SaxsSpot.NanoSystemGeneration.Engine.Models.TernaryTree;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public static class NanosystemAnalyzer
{
	public static ICollection<ZoneConcentrationAnalyze> 
		GetNanosystemAnalyze<T>
		(ICollection<T> particles, GenerationZone generationZone, int zoneCount, int vectorCount) where T : Particle
	{
		// V = 4/3*pi*r^3
		// r = (V/(4/3*pi))^1/3
		var boundCount = zoneCount + 1;
		var globalVolume = generationZone.GetInnerSphereVolume();
		var volumeStep = globalVolume / (boundCount-1);

		var currentRadius = 0d;
		var radii = new List<double>(boundCount);
		for(var i = 0; i < (boundCount-1); i++)
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

		var tree = new TernaryTreeNode(particles.MaxBy(x => x.GetDiameter())!.GetDiameter(),
			(generationZone.GenerationZoneForm == GenerationZoneForm.Cube
				? generationZone.GlobalSize
				: generationZone.GlobalSize * 2) + 1);
		
		foreach (var particle in particles)
		{
			tree.InsertParticle(particle);
		}

    	var result = new ConcurrentBag<ZoneConcentrationAnalyze>();
	    var points = RandomVectorGenerator.GenerateRandomVectors(
		    vectorCount, generationZone
	    );
	    Parallel.ForEach(bounds.OrderBy(p => p.ZoneIndex), new ParallelOptions()
	    {
		    MaxDegreeOfParallelism = Environment.ProcessorCount
	    }, bound =>
	    {
		    var currentPoints = points.Where(x => x.L2Norm() >= bound.InnerRadius && x.L2Norm() <= bound.OuterRadius);
		    
		    var pointsInParticle = 0f;
		    var inBound = particles.Where(x =>
			    IntersectionService.IsParticleBelongsZone(x, bound.OuterRadius, bound.OuterRadius - bound.InnerRadius));

		    if (inBound.FirstOrDefault() is Sphere)
		    {
			    foreach (var point in currentPoints)
			    {
				    var node = tree.FindDeepestNodeForVector(tree, point);

				    var currentParticles = node.GetParticles().Union(node._neighbors.SelectMany(x => x.GetParticles()));
				    foreach (var paricle in currentParticles)
				    {
					    if (IntersectionService.IsPointInSphere(point, paricle))
					    {
						    pointsInParticle++;
					    }
				    }
			    }

		    }
		    else if (inBound.FirstOrDefault() is Parallelepiped)
		    {
			    foreach (var point in currentPoints)
			    {
				    var node = tree.FindDeepestNodeForVector(tree, point);

				    var currentParticles = node.GetParticles().Union(node._neighbors.SelectMany(x => x.GetParticles()));
				    foreach (var paricle in currentParticles)
				    {
					    if (IntersectionService.IsPointInParallelepiped(point, paricle))
					    {
						    pointsInParticle++;
					    }
				    }
			    }
		    }

		    result.Add(new ZoneConcentrationAnalyze(bound.ZoneIndex, pointsInParticle / currentPoints.Count()));
	    });
    	
    	return result.OrderBy(x => x.ZoneIndex).ToList();
    }
}