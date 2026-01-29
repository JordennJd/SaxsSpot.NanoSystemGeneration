using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationInfo;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using static Extreme.Mathematics.Elementary;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal static class IntersectionService
{
    /// <summary>
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <param name="density">The density of the points to check.</param>
	/// <returns>True if the two parallelepipeds intersect, false otherwise.</returns>
	public static bool IsIntersect(Parallelepiped oldPar, Parallelepiped newPar, int density = 10, bool isNeighbors = false, GenerationInfo? info = null, ParticleGenerationInfo? particleInfo = null)
	{
		info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
		particleInfo?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
		if (IsInterCenterDistanceMoreThenDiagonalCheck(oldPar, newPar))
		{
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
			particleInfo?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
			return false;
		}
		
		info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesTotal();
		particleInfo?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesTotal();
		if (IsInterCenterDistanceLessThenSidesCheck(oldPar, newPar))
		{
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesPositive();
			particleInfo?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesPositive();
			return true;
		}
		
		info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal();
		particleInfo?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal();
		var newCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(newPar).Copy();
		ParallelepipedManipulator.DoParallelepipedTransform(ref newCord, -oldPar.X, -oldPar.Y, -oldPar.Z);
		ParallelepipedManipulator.DoBackParallelepipedRotate(ref newCord, oldPar, info, particleInfo);
		if (ElementaryIntersectCheckOnlyBorders(oldPar, newCord))
		{
			info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive();
			particleInfo?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive();
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			return true;
		}
		
		info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal();
		particleInfo?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal();
		var oldCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(oldPar).Copy();
		ParallelepipedManipulator.DoParallelepipedTransform(ref oldCord, -newPar.X, -newPar.Y, -newPar.Z);
		ParallelepipedManipulator.DoBackParallelepipedRotate(ref oldCord, newPar, info, particleInfo);
		if (ElementaryIntersectCheckOnlyBorders(newPar, oldCord))
		{
			info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive();
			particleInfo?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive();
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			return true;
		}

		if (SAT.IsIntersect(oldPar, newPar, info, particleInfo))
		{
			return true;
		}
		//
		// var surfacesOld = ParallelepipedCoverer.FillBorders(oldPar, oldCord, density);
		// if (HardIntersectCheckOnlyBorders(newPar, surfacesOld))
		// {
		// 	newPar.BackRotateMatrix = null;
		// 	newPar.IsEdgesRotated = false;
		// 	newPar.Edges = null;
		// 	newPar.Borders = null;
		// 	return true;
		// }
		
		return false;
	}
	
	public static IEnumerable<Parallelepiped> FindParallelepipedsByPoint(Vector<float> vector,
		IEnumerable<Parallelepiped> parallelepipeds)
	{
		var list = new List<Parallelepiped>();
		foreach (var par in parallelepipeds)
		{
			var vec = Vector<float>.Build.DenseOfArray([vector[0], vector[1], vector[2]]);

			if (Sqrt(Pow(par.X - vector[0], 2) + Pow(par.Y - vector[1], 2) + Pow(par.Z - vector[2], 2)) >
				Sqrt(Pow(par.A, 2) + Pow(par.A, 2) + Pow(par.A * par.E, 2))) continue;
			vec = ParallelepipedManipulator.DoBackVectorRotate(
				ParallelepipedManipulator.DoVectorTransform(vec, -par.X, -par.Y, -par.Z), par.Phi, par.Theta,
				par.Zenit);

			if (IsVectorInBounds(vec, par)) list.Add(par);
			;
		}

		return list;
	}

	public static bool IsPointInParallelepiped(Vector<float> vector, Particle parallelepiped)
	{
		var par = parallelepiped as Parallelepiped;
		var outerRadius = (Pow(par.A/2, 2) + Pow(par.A/2, 2) + Pow(par.A/2 * par.E, 2));
		var innerRadius = par.A/2;

		var dx = Pow(par.X - vector[0], 2);
		var dy = Pow(par.Y - vector[1], 2);
		var dz = Pow(par.Z - vector[2], 2);
		if (dx + dy + dz > outerRadius)
			return false;
		if (Sqrt(dx + dy + dz) < innerRadius)
			return true;
		
		var vec = Vector<float>.Build.DenseOfArray([vector[0], vector[1], vector[2]]);
		vec = ParallelepipedManipulator
			.DoBackVectorRotate(ParallelepipedManipulator
					.DoVectorTransform(vec, -par.X, -par.Y, -par.Z), par.Phi,
		par.Theta, par.Zenit, par.BackRotateMatrix);

		if (IsVectorInBounds(vec, par))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public static bool IsPointInSphere(Vector<float> vector, Particle particle)
	{
		var sphere = particle as Sphere;
		return Pow(sphere.X - vector[0], 2) + Pow(sphere.Y - vector[1], 2) + Pow(sphere.Z - vector[2], 2) <= sphere.Radius*sphere.Radius;
	}

	/// <summary>
	///     Checks if the new parallelepiped intersects with the old parallelepiped.
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <returns>True if the parallelepipeds intersect, false otherwise.</returns>

	private static bool IsPointInParallelepipedCheckForCube(Vector<float> vector, Parallelepiped parallelepiped)
	{
		if (MathF.Abs(parallelepiped.E - 1) < 0.0001)
		{
			if (Sqrt(Pow(parallelepiped.X - vector[0], 2) + Pow(parallelepiped.Y - vector[1], 2) +
					 Pow(parallelepiped.Z - vector[2], 2)) >= parallelepiped.A / 2 * 1.73205080757) return false;
			if (Sqrt(Pow(parallelepiped.X - vector[0], 2) + Pow(parallelepiped.Y - vector[1], 2) +
					 Pow(parallelepiped.Z - vector[2], 2)) <= parallelepiped.A / 2) return true;

			throw new ArgumentException("impossible to determine");
		}

		throw new ArgumentException("is not a cube");
	}

	private static bool HardIntersectCheckOnlyBorders(Parallelepiped par, Vector<float>[][] borders)
	{
		return borders
			.Any(border => border
				.Any(vector => IsVectorInBounds(vector, par)));
	}

	private static async Task<bool> HardIntersectCheck(Parallelepiped par, List<List<List<Vector<float>>>> Surfaces)
	{
		var listTasks = new List<Task<bool>>();
		foreach (var col in Surfaces)
			listTasks.Add(Task.Run(() =>
			{
				foreach (var row in col)
				foreach (var Vector in row)
					if (IsVectorInBounds(Vector, par))
						return true;
				return false;
			}));
		var results = await Task.WhenAll(listTasks);
		foreach (var result in results)
			if (result)
				return true;
		return false;
	}
	
	public static bool ElementaryIntersectCheckOnlyBorders(Parallelepiped par, ParallelepipedCoordinates cords)
	{
		foreach (var edge in cords.ForAll())
			if (IsVectorInBounds(edge, par))
				return true;
		return false;
	}

	/// <summary>
	///     Checks if the inter-center distance between two parallelepipeds is greater than the diagonal check.
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <returns>True if the inter-center distance is greater than the diagonal check, false otherwise.</returns>
	public static bool IsInterCenterDistanceMoreThenDiagonalCheck(Parallelepiped oldPar, Parallelepiped newPar)
	{
		var interCenterDistance =
			Sqrt(Pow(oldPar.X - newPar.X, 2) + Pow(oldPar.Y - newPar.Y, 2) + Pow(oldPar.Z - newPar.Z, 2));

		var oldDiagonal = Sqrt(Pow(oldPar.A, 2) + Pow(oldPar.A, 2) + Pow(oldPar.A * oldPar.E, 2));
		var newDiagonal = Sqrt(Pow(newPar.A, 2) + Pow(newPar.A, 2) + Pow(newPar.A * newPar.E, 2));

		return interCenterDistance > oldDiagonal / 2 + newDiagonal / 2;
	}
	
	public static bool IsInterCenterDistanceMoreThenDiagonalCheckForNodes(Parallelepiped oldPar, Parallelepiped newPar)
	{
		var interCenterDistance =
			Sqrt(Pow(oldPar.X - newPar.X, 2) + Pow(oldPar.Y - newPar.Y, 2) + Pow(oldPar.Z - newPar.Z, 2));

		var oldDiagonal = Sqrt(Pow(oldPar.A, 2) + Pow(oldPar.A, 2) + Pow(oldPar.A * oldPar.E, 2));
		var newDiagonal = Sqrt(Pow(newPar.A, 2) + Pow(newPar.A, 2) + Pow(newPar.A * newPar.E, 2));

		var buffer = Max(oldDiagonal, newDiagonal);
		
		return interCenterDistance > (oldDiagonal / 2 + newDiagonal / 2) + buffer;
	}
	
	public static bool IsInterCenterDistanceMoreThenDiagonalCheckForNodesSphere(Sphere oldPar, Parallelepiped newPar)
	{
		var interCenterDistance =
			Sqrt(Pow(oldPar.X - newPar.X, 2) + Pow(oldPar.Y - newPar.Y, 2) + Pow(oldPar.Z - newPar.Z, 2));

		var oldDiagonal = oldPar.Radius;
		var newDiagonal = Sqrt(Pow(newPar.A, 2) + Pow(newPar.A, 2) + Pow(newPar.A * newPar.E, 2));

		var buffer = Min(oldDiagonal, newDiagonal/2) / 1.9f;

		return interCenterDistance > (oldDiagonal + newDiagonal / 2) + buffer;
	}
	
	/// <summary>
	///     Checks if the inter-center distance between two parallelepipeds is smaller than the sides sum check.
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <returns>True if the inter-center distance is greater than the diagonal check, false otherwise.</returns>
	public static bool IsInterCenterDistanceLessThenSidesCheck(Parallelepiped oldPar, Parallelepiped newPar)
	{
		var interCenterDistance =
			Sqrt(Pow(oldPar.X - newPar.X, 2) + Pow(oldPar.Y - newPar.Y, 2) + Pow(oldPar.Z - newPar.Z, 2));

		var oldSide = oldPar.A * oldPar.E;
		var newSide = newPar.A * newPar.E;

		return interCenterDistance < newSide / 2 + oldSide / 2;
	}
	
	/// <summary>
	/// Checks if the inter-center distance between two parallelepipeds is smaller than the sides sum check.
	/// </summary>
	/// <param name="s1"></param>
	/// <param name="s2"></param>
	/// <returns>True if the inter-center distance is greater than the diagonal check, false otherwise.</returns>
	public static bool IsSphereIntersect(Sphere s1, Sphere s2, GenerationInfo? info = null, ParticleGenerationInfo? particleInfo = null)
	{
		var interCenterDistance =
			Sqrt(Pow(s1.X - s2.X, 2) + Pow(s1.Y - s2.Y, 2) + Pow(s1.Z - s2.Z, 2));

		info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
		particleInfo?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
		if (interCenterDistance < s1.Radius + s2.Radius)
		{
			info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
			particleInfo?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
			return true;
		}
		
		return false;
	}

	public static bool IsVectorBelongsZone(Vector<float> vector, double outerBound, double step)
	{
		var distance = Sqrt(Pow(vector[0], 2) + Pow(vector[1], 2) + Pow(vector[2], 2));
		
		return distance < outerBound && distance > outerBound - step;
	}
	
	public static bool IsParticleBelongsZone(Particle particle, double outerBound, double step)
	{
		var innerBound = outerBound - step;
    
		if (particle.ParticleKind == ParticleKind.Sphere)
		{
			var centerDistance = Sqrt(Pow(particle.X, 2) + Pow(particle.Y, 2) + Pow(particle.Z, 2));
			var radius = ((Sphere)particle).Radius;
			
			return (centerDistance + radius) >= innerBound && (centerDistance - radius) <= outerBound;
		}

		if (particle.ParticleKind == ParticleKind.Parallelepiped)
		{
			var par = particle as Parallelepiped;
			var centerDistance = Sqrt(Pow(particle.X, 2) + Pow(particle.Y, 2) + Pow(particle.Z, 2));
			var innerRadius = par.A/2; //TODO if E == 1
			
			if (outerBound > centerDistance - innerRadius && innerBound < centerDistance + innerRadius)
			{
				return true;
			}
			
			var edges = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates((Parallelepiped)particle);
			var borders = ParallelepipedCoverer.FillBorders((Parallelepiped)particle, edges, 5);
			foreach (var edge in borders.SelectMany(x => x))
			{
				var distance = Sqrt(Pow(edge[0], 2) + Pow(edge[1], 2) + Pow(edge[2], 2));
			
				if (distance > innerBound && distance < outerBound)
				{
					return true;
				}
			}
        
			return false;
		}
    
		return false;
	}
	
	public static bool IsSphereInBoundCube(Sphere s1, Parallelepiped cubeZone)
	{
		float sphereX = s1.X;
		float sphereY = s1.Y;
		float sphereZ = s1.Z;
		float radius = s1.Radius;
    
		float cubeX = cubeZone.X;
		float cubeY = cubeZone.Y;
		float cubeZ = cubeZone.Z;
		float cubeA = cubeZone.A;
		float cubeE = cubeZone.E;
    
		bool insideX = (sphereX - radius >= cubeX - cubeA/2) && (sphereX + radius <= cubeX + cubeA/2);
		bool insideY = (sphereY - radius >= cubeY - cubeE/2) && (sphereY + radius <= cubeY + cubeE/2);
		bool insideZ = (sphereZ - radius >= cubeZ - cubeE/2) && (sphereZ + radius <= cubeZ + cubeE/2);
    
		return insideX && insideY && insideZ;
	}

	public static bool IsParticleInsideCubeZoneSphere(Sphere particle, GenerationZone zone)
	{
		return IsSphereInBoundsOfCubeZone(zone.GlobalSize, particle);
	}
	
	public static bool IsParticleInsideSphereCubeZoneSphere(Sphere particle, GenerationZone zone)
	{
		return IsSphereInBoundsOfSphereZone(zone.GlobalSize, particle);
	}
	
	public static bool IsParticleInsideZoneParallelepiped(Parallelepiped particle, GenerationZone zone)
	{
		return IsParallelepipedInBoundsOfCubeZone(zone.GlobalSize, particle);
	}
	
	public static bool IsParallelepipedInBoundsOfCubeZone(double globalSize, Parallelepiped parallelepiped)
	{
		var par = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(parallelepiped);
		if (IsVectorInBoundsCube(par.A, globalSize) && IsVectorInBoundsCube(par.B, globalSize) &&
		    IsVectorInBoundsCube(par.C, globalSize)
		    && IsVectorInBoundsCube(par.D, globalSize) && IsVectorInBoundsCube(par.A1, globalSize) &&
		    IsVectorInBoundsCube(par.B1, globalSize)
		    && IsVectorInBoundsCube(par.C1, globalSize) && IsVectorInBoundsCube(par.D1, globalSize))
		{
			return true;
		}
		parallelepiped.BackRotateMatrix = null;
		parallelepiped.IsEdgesRotated = false;
		parallelepiped.Edges = null;
		return false;
	}
	
	public static bool IsParallelepipedInBoundsOfSphereZone(double globalRadius, Parallelepiped parallelepiped)
	{
		return Sqrt(Pow(parallelepiped.X, 2) + Pow(parallelepiped.Y, 2) + Pow(parallelepiped.Z, 2)) <= globalRadius;

	}

	public static bool IsSphereInBoundsOfSphereZone(double globalRadius, Sphere sphere)
	{
		return Sqrt(Pow(sphere.X, 2) + Pow(sphere.Y, 2) + Pow(sphere.Z, 2)) <= globalRadius;
	}
	
	public static bool IsSphereInBoundsOfCubeZone(double globalRadius, Sphere sphere)
	{
		var halfExtent = globalRadius / 2;

		if (sphere.X - sphere.Radius < -halfExtent || sphere.X + sphere.Radius > halfExtent)
			return false;
		if (sphere.Y - sphere.Radius < -halfExtent || sphere.Y + sphere.Radius > halfExtent)
			return false;
		if (sphere.Z - sphere.Radius < -halfExtent || sphere.Z + sphere.Radius > halfExtent)
			return false;

		return true;
	}

	public static bool IsPointInsideParticle(Vector<float>? vector, Particle particle)
	{
		if (vector is null) return false;
		
		switch (particle.ParticleKind)
		{
			case ParticleKind.Sphere:
				return IsPointInSphere(vector, particle);
			case ParticleKind.Parallelepiped:
				return IsPointInParallelepiped(vector, particle);
			default:
				throw new ArgumentException("Not supported check");
		}
	}
	
	private static bool IsVectorInBoundsCube(Vector<float> vector, double globalSize)
	{
		return vector[0] <= globalSize / 2 && vector[0] >= -globalSize / 2 &&
		       vector[1] <= globalSize / 2 && vector[1] >= -globalSize / 2 &&
		       vector[2] <= globalSize / 2 && vector[2] >= -globalSize / 2;
	}
	
	private static bool IsVectorInBoundsSphere(Vector<float> vector, double radius)
	{
		return vector[0]*vector[0] + vector[1]*vector[1] + vector[2]*vector[2] <= radius * radius;
	}
	
	/// <summary>
	///     Checks if a vector is within the bounds of a parallelepiped.
	/// </summary>
	/// <param name="vector">The vector to check.</param>
	/// <param name="parallelepiped">The parallelepiped to compare against.</param>
	/// <returns>True if the vector is within the bounds, false otherwise.</returns>
	private static bool IsVectorInBounds(Vector<float> vector, Parallelepiped parallelepiped)
	{
		return vector[0] <= parallelepiped.A / 2 && vector[0] >= -parallelepiped.A / 2 &&
		       vector[1] <= parallelepiped.A / 2 && vector[1] >= -parallelepiped.A / 2 &&
		       vector[2] <= parallelepiped.A / 2 * parallelepiped.E && vector[2] >= -parallelepiped.A / 2 * parallelepiped.E;
	}
	
	/// <summary>
	///     Checks if a vector is within the bounds of a parallelepiped.
	/// </summary>
	/// <param name="vector">The vector to check.</param>
	/// <param name="parallelepiped">The parallelepiped to compare against.</param>
	/// <returns>True if the vector is within the bounds, false otherwise.</returns>
	private static bool IsVectorInBounds(Vector<float> vector, Sphere sphere)
	{
		var radius = sphere.Radius;
		
		return vector[0] <= radius && vector[0] >= -radius &&
		       vector[1] <= radius && vector[1] >= -radius &&
		       vector[2] <= radius && vector[2] >= -radius;
	}
	
	public static float Sqrt(float _d) 
	{
		float w = _d, h = 1f;

		if (w < 1f)
		{
			h = _d;
			w = 1f;
		}

		do
		{
			w *= 0.5f;
			h += h;
		} while (w > h);

		float x = h + w;
		float x2 = x * x;
		float x4 = x2 * x * x;
		float x6 = x4 * x * x;
		float x8 = x6 * x * x;
		float h2 = h * h;
		float h3 = h2 * h;
		float h4 = h3 * h;
		float w2 = w * w;
		float w3 = w2 * w;
		float w4 = w3 * w;
		float hw = h * w;
		float h2w2 = h2 * w2;
		float a = (256f * h4 * w4 + 1792 * h3 * w3 * x2 + 1120 * h2w2 * x4 + 112 * hw * x6 + x8);
		float b = (16f * h2w2 + 24 * hw * x2 + x4);
		float c = (4f * hw + x2);
		float xcb = x * c * b;
		return (8f * hw * xcb) / a + a / (32f * xcb);
	}
}