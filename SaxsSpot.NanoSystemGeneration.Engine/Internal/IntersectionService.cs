using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using static System.Math;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal static class IntersectionService
{
    /// <summary>
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <param name="density">The density of the points to check.</param>
	/// <returns>True if the two parallelepipeds intersect, false otherwise.</returns>
	public static bool IsIntersect(Parallelepiped oldPar, Parallelepiped newPar, int density = 10)
	{
		if (oldPar.X == newPar.X && oldPar.Y == newPar.Y && oldPar.Z == newPar.Z) return true;


		if (IsInterCenterDistanceMoreThenDiagonalCheck(oldPar, newPar)) return false;
		if (IsInterCenterDistanceLessThenSidesCheck(oldPar, newPar)) return true;

		// return SAT.AreCubesIntersecting(oldPar, newPar);
		
		var newCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(newPar);
		ParallelepipedManipulator.DoParallelepipedTransform(ref newCord, -oldPar.X, -oldPar.Y, -oldPar.Z);
		ParallelepipedManipulator.DoBackParallelepipedRotate(ref newCord, oldPar.Phi, oldPar.Theta, oldPar.Zenit);
		var surfacesNew = ParallelepipedCoverer.FillBorders(newCord, density);


		if (ElementaryIntersectCheckOnlyBorders(oldPar, surfacesNew)) return true;


		var oldCord = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(oldPar);
		ParallelepipedManipulator.DoParallelepipedTransform(ref oldCord, -newPar.X, -newPar.Y, -newPar.Z);
		ParallelepipedManipulator.DoBackParallelepipedRotate(ref oldCord, newPar.Phi, newPar.Theta, newPar.Zenit);
		var surfacesOld = ParallelepipedCoverer.FillBorders(oldCord, density);

		if (ElementaryIntersectCheckOnlyBorders(newPar, surfacesOld)) return true;

		if (HardIntersectCheckOnlyBorders(oldPar, surfacesNew)) return true;
		if (HardIntersectCheckOnlyBorders(newPar, surfacesOld)) return true;
		
		surfacesNew.Clear();
		surfacesOld.Clear();
		
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

	private static bool HardIntersectCheckOnlyBorders(Parallelepiped par, List<List<Vector<float>>> borders)
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

	private static bool ElementaryIntersectCheck(Parallelepiped par, List<List<List<Vector<float>>>> Surfaces)
	{
		foreach (var Planes in Surfaces)
			if (IsVectorInBounds(Planes[0][0], par) || IsVectorInBounds(Planes[^1][0], par)
													|| IsVectorInBounds(Planes[0][^1], par) ||
													IsVectorInBounds(Planes[^1][^1], par))
				return true;
		return false;
	}

	private static bool ElementaryIntersectCheckOnlyBorders(Parallelepiped par, List<List<Vector<float>>> Borders)
	{
		foreach (var Border in Borders)
			if (IsVectorInBounds(Border[0], par) || IsVectorInBounds(Border[^1], par))
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
			Math.Sqrt(Math.Pow(oldPar.X - newPar.X, 2) + Math.Pow(oldPar.Y - newPar.Y, 2) + Math.Pow(oldPar.Z - newPar.Z, 2));

		var oldDiagonal = Math.Sqrt(Math.Pow(oldPar.A, 2) + Math.Pow(oldPar.A, 2) + Math.Pow(oldPar.A * oldPar.E, 2));
		var newDiagonal = Math.Sqrt(Math.Pow(newPar.A, 2) + Math.Pow(newPar.A, 2) + Math.Pow(newPar.A * newPar.E, 2));

		return interCenterDistance > oldDiagonal / 2 + newDiagonal / 2;
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
			Math.Sqrt(Math.Pow(oldPar.X - newPar.X, 2) + Math.Pow(oldPar.Y - newPar.Y, 2) + Math.Pow(oldPar.Z - newPar.Z, 2));

		var oldSide = oldPar.A * oldPar.E;
		var newSide = oldPar.A * oldPar.E;

		return interCenterDistance < newSide / 2 + oldSide / 2;
	}
	
	/// <summary>
	/// Checks if the inter-center distance between two parallelepipeds is smaller than the sides sum check.
	/// </summary>
	/// <param name="oldPar">The old parallelepiped.</param>
	/// <param name="newPar">The new parallelepiped.</param>
	/// <returns>True if the inter-center distance is greater than the diagonal check, false otherwise.</returns>
	public static bool IsSphereIntersect(Sphere s1, Sphere s2)
	{
		var interCenterDistance =
			Math.Sqrt(Math.Pow(s1.X - s2.X, 2) + Math.Pow(s1.Y - s2.Y, 2) + Math.Pow(s1.Z - s2.Z, 2));

		return interCenterDistance < s1.Radius + s2.Radius;
	}

	/// <summary>
	///     Checks if a vector is within the bounds of a parallelepiped.
	/// </summary>
	/// <param name="Vector">The vector to check.</param>
	/// <param name="oldPar">The parallelepiped to compare against.</param>
	/// <returns>True if the vector is within the bounds, false otherwise.</returns>
	private static bool IsVectorInBounds(Vector<float> Vector, Parallelepiped oldPar)
	{
		return Vector[0] <= oldPar.A / 2 && Vector[0] >= -oldPar.A / 2 &&
			   Vector[1] <= oldPar.A / 2 && Vector[1] >= -oldPar.A / 2 &&
			   Vector[2] <= oldPar.A / 2 * oldPar.E && Vector[2] >= -oldPar.A / 2 * oldPar.E;
	}

	public static bool IsParallelepipedInBoundsOfZone(float size, Parallelepiped parallelepiped)
	{
		var par = ParallelepipedManipulator.ParallelepipedToParallelepipedCoordinates(parallelepiped);
		var cubeZone = new Parallelepiped(size, 1);
		if (IsVectorInBounds(par.A, cubeZone) && IsVectorInBounds(par.B, cubeZone) && IsVectorInBounds(par.C, cubeZone)
			&& IsVectorInBounds(par.D, cubeZone) && IsVectorInBounds(par.A1, cubeZone) &&
			IsVectorInBounds(par.B1, cubeZone)
			&& IsVectorInBounds(par.C1, cubeZone) && IsVectorInBounds(par.D1, cubeZone)
		   )
			return true;
		return false;
	}
}