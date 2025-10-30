using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using static System.MathF;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal static class IntersectionService
{
    /// <summary>
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <param name="density">The density of the points to check.</param>
	/// <returns>True if the two parallelepipeds intersect, false otherwise.</returns>
	public static bool IsIntersect(Parallelepiped oldPar, Parallelepiped newPar, int density = 10, bool isNeighbors = false, GenerationInfo? info = null)
	{
		info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
		if (IsInterCenterDistanceMoreThenDiagonalCheck(oldPar, newPar))
		{
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
			return false;
		}
		
		info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesTotal();
		if (IsInterCenterDistanceLessThenSidesCheck(oldPar, newPar))
		{
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			info?.IncrementIsInterCenterDistanceLessThenSidesCheckTimesPositive();
			return true;
		}
		
		info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal();
		var newCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(newPar).Copy();
		ParallelepipedManipulator.DoParallelepipedTransform(ref newCord, -oldPar.X, -oldPar.Y, -oldPar.Z);
		ParallelepipedManipulator.DoBackParallelepipedRotate(ref newCord, oldPar);
		if (ElementaryIntersectCheckOnlyBorders(oldPar, newCord))
		{
			info?.IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive();
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			return true;
		}
		
		info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal();
		var oldCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(oldPar).Copy();
		ParallelepipedManipulator.DoParallelepipedTransform(ref oldCord, -newPar.X, -newPar.Y, -newPar.Z);
		ParallelepipedManipulator.DoBackParallelepipedRotate(ref oldCord, newPar);
		if (ElementaryIntersectCheckOnlyBorders(newPar, oldCord))
		{
			info?.IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive();
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			newPar.IsParticleInside = false;
			return true;
		}
		
		var surfacesOld = ParallelepipedCoverer.FillBorders(oldPar, oldCord, density);
		if (HardIntersectCheckOnlyBorders(newPar, surfacesOld))
		{
			newPar.BackRotateMatrix = null;
			newPar.IsEdgesRotated = false;
			newPar.Edges = null;
			newPar.Borders = null;
			return true;
		}
		
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

	public static int GetCountOfParallelepipedsByPoint(Vector<float> vector, IEnumerable<Parallelepiped> parallelepipeds)
	{
		var c = 0;
		foreach (var par in parallelepipeds)
		{
			try
			{
				if (IsPointInParallelepipedCheckForCube(vector, par)) c++;
				else continue;
			}
			catch
			{
			}

			if (Sqrt(Pow(par.X - vector[0], 2) + Pow(par.Y - vector[1], 2) + Pow(par.Z - vector[2], 2)) >
				Sqrt(Pow(par.A, 2) + Pow(par.A, 2) + Pow(par.A * par.E, 2))) continue;
			var vec = Vector<float>.Build.DenseOfArray([vector[0], vector[1], vector[2]]);
			vec = ParallelepipedManipulator.DoBackVectorRotate(
				ParallelepipedManipulator.DoVectorTransform(vec, -par.X, -par.Y, -par.Z), par.Phi, par.Theta,
				par.Zenit);

			if (IsVectorInBounds(vec, par)) c++;
			;
		}

		return c;
	}

	public static bool IsPointInParallelepiped(Vector<float> vector, Particle parallelepiped)
	{
		var par = parallelepiped as Parallelepiped;
		if (Sqrt(Pow(par.X - vector[0], 2) + Pow(par.Y - vector[1], 2) +
				 Pow(par.Z - vector[2], 2)) > Sqrt(Pow(par.A, 2) + Pow(par.A, 2) + Pow(par.A * par.E, 2)))
			return false;
		
		var vec = Vector<float>.Build.DenseOfArray([vector[0], vector[1], vector[2]]);
		vec = ParallelepipedManipulator
			.DoBackVectorRotate(ParallelepipedManipulator
					.DoVectorTransform(vec, -par.X, -par.Y, -par.Z), par.Phi,
		par.Theta, par.Zenit);

		return IsVectorInBounds(vec, par);
	}
	
	public static bool IsPointInSphere(Vector<float> vector, Particle particle)
	{
		var sphere = particle as Sphere;
		return Sqrt(Pow(sphere.X - vector[0], 2) + Pow(sphere.Y - vector[1], 2) + Pow(sphere.Z - vector[2], 2)) < sphere.Radius;
	}

	/// <summary>
	///     Checks if the new parallelepiped intersects with the old parallelepiped.
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <returns>True if the parallelepipeds intersect, false otherwise.</returns>

	private static bool IsPointInParallelepipedCheckForCube(Vector<float> vector, Parallelepiped parallelepiped)
	{
		if (Abs(parallelepiped.E - 1) < 0.0001)
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
	
	private static bool ElementaryIntersectCheckOnlyBorders(Parallelepiped par, ParallelepipedCoordinates cords)
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

		var buffer = Min(oldDiagonal, newDiagonal/2) / 1.5f;

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
	public static bool IsSphereIntersect(Sphere s1, Sphere s2, GenerationInfo? info = null)
	{
		var interCenterDistance =
			Sqrt(Pow(s1.X - s2.X, 2) + Pow(s1.Y - s2.Y, 2) + Pow(s1.Z - s2.Z, 2));

		info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal();
		if (interCenterDistance < s1.Radius + s2.Radius)
		{
			info?.IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive();
			return true;
		}
		
		return interCenterDistance < s1.Radius + s2.Radius;
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

	public static bool IsParticleInsideZone(Particle particle, GenerationZone zone)
	{
		switch (zone.GenerationZoneForm)
		{
			case GenerationZoneForm.Cube when particle.ParticleKind == ParticleKind.Parallelepiped:
				return IsParallelepipedInBoundsOfCubeZone(zone.GlobalSize, particle as Parallelepiped);
			
			case GenerationZoneForm.Cube when particle.ParticleKind == ParticleKind.Sphere:
				return IsSphereInBoundsOfCubeZone(zone.GlobalSize, particle as Sphere);
			
			case GenerationZoneForm.Sphere when particle.ParticleKind == ParticleKind.Parallelepiped:
				return IsParallelepipedInBoundsOfSphereZone(zone.GlobalSize, particle as Parallelepiped);
			
			case GenerationZoneForm.Sphere when particle.ParticleKind == ParticleKind.Sphere:
				return IsSphereInBoundsOfSphereZone(zone.GlobalSize, particle as Sphere);
			
			default:
				throw new ArgumentException("not supported check");
		}
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
		var par = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(parallelepiped);

		return IsVectorInBoundsSphere(par.A, globalRadius) && IsVectorInBoundsSphere(par.B, globalRadius) &&
		       IsVectorInBoundsSphere(par.C, globalRadius)
		       && IsVectorInBoundsSphere(par.D, globalRadius) && IsVectorInBoundsSphere(par.A1, globalRadius) &&
		       IsVectorInBoundsSphere(par.B1, globalRadius)
		       && IsVectorInBoundsSphere(par.C1, globalRadius) && IsVectorInBoundsSphere(par.D1, globalRadius);
	}

	public static bool IsSphereInBoundsOfSphereZone(double globalRadius, Sphere sphere)
	{
		return sphere.Radius + Sqrt(Pow(sphere.X, 2) + Pow(sphere.Y, 2) + Pow(sphere.Z, 2)) <= globalRadius;
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
}