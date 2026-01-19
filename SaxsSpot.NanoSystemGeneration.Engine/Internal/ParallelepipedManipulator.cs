using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationInfo;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal static class ParallelepipedManipulator
{
    /// <summary>
	/// Converts a Parallelepiped object to its corresponding ParallelepipedCoordinates.
	/// </summary>
	/// <param name="par">The Parallelepiped object to convert.</param>
	/// <returns>The ParallelepipedCoordinates object.</returns>
	public static ParallelepipedCoordinates ParallelepipedToParallelepipedCoordinates(Parallelepiped par)
	{
		if (par.Edges is null)
		{
			PrepareParallelepiped(par);
		}

		var cords = new ParallelepipedCoordinates(par.Edges[0], par.Edges[1], par.Edges[2], par.Edges[3], par.Edges[4],
			par.Edges[5], par.Edges[6], par.Edges[7]);

		return cords;
	}

	public static void PrepareParallelepiped(Parallelepiped par)
	{
		if (par.Edges is null)
		{
			par.Edges = new List<Vector<float>>(8);
			var A = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 + par.A / 2, 0 + par.A * par.E / 2]);
			var B = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 + par.A / 2, 0 + par.A * par.E / 2]);
			var C = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 + par.A / 2, 0 - par.A * par.E / 2]);
			var D = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 + par.A / 2, 0 - par.A * par.E / 2]);
			var A1 = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 - par.A / 2, 0 + par.A * par.E / 2]);
			var B1 = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 - par.A / 2, 0 + par.A * par.E / 2]);
			var C1 = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 - par.A / 2, 0 - par.A * par.E / 2]);
			var D1 = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 - par.A / 2, 0 - par.A * par.E / 2]);
			
			par.Edges!.AddRange([A, B, C, D, A1, B1, C1, D1]);
			var matrix = DoParallelepipedRotate(par.Edges, par.Phi, par.Theta, par.Zenit);
			par.BackRotateMatrix = matrix.Transpose();
			DoParallelepipedTransform(par.Edges, par.X, par.Y, par.Z);
		}
	}

	/// <summary>
	///     Rotates a parallelepiped by the specified angles.
	/// </summary>
	/// <param name="par">The coordinates of the parallelepiped.</param>
	/// <param name="Fi">The rotation angle around the X-axis.</param>
	/// <param name="Theta">The rotation angle around the Y-axis.</param>
	/// <param name="Zenit">The rotation angle around the Z-axis.</param>
	public static Matrix<float> DoParallelepipedRotate(List<Vector<float>> edges, float Fi, float Theta,
		float Zenit)
	{
		var xRotate = Matrix<float>.Build.DenseOfArray(new[,]
			{
				{ 1, 0, 0 },
				{ 0, MathF.Cos(Fi), -MathF.Sin(Fi) },
				{ 0, MathF.Sin(Fi), MathF.Cos(Fi) }
			}
		);
		
		var yRotate = Matrix<float>.Build.DenseOfArray(new[,]
			{
				{ MathF.Cos(Theta), 0, MathF.Sin(Theta) },
				{ 0, 1, 0 },
				{ -MathF.Sin(Theta), 0, MathF.Cos(Theta) }
			}
		);
		
		var zRotate = Matrix<float>.Build.DenseOfArray(new[,]
		{
			{ MathF.Cos(Zenit), -MathF.Sin(Zenit), 0 },
			{ MathF.Sin(Zenit), MathF.Cos(Zenit), 0 },
			{ 0, 0, 1 }
		});

		var rotateMatrix = xRotate * yRotate * zRotate;

		for (int i = 0; i < edges.Count; i++)
		{
			edges[i] = 	DoBackVectorRotate(edges[i], rotateMatrix);
		}

		return rotateMatrix;
	}

	/// <summary>
	///     Performs a back rotation on the given parallelepiped coordinates.
	/// </summary>
	/// <param name="parallelepipedCoordinates">The coordinates of the parallelepiped.</param>
	/// <param name="Fi">The rotation angle in the xy-plane.</param>
	/// <param name="Theta">The rotation angle in the xz-plane.</param>
	/// <param name="Zenit">The rotation angle in the yz-plane.</param>
	public static void DoBackParallelepipedRotate(ref ParallelepipedCoordinates parallelepipedCoordinates, Parallelepiped par, GenerationInfo? info = null, ParticleGenerationInfo? particleInfo = null)
	{
		if (par.BackRotateMatrix is null)
		{
			var xRotate = Matrix<float>.Build.DenseOfArray(new[,]
				{
					{ 1, 0, 0 },
					{ 0, MathF.Cos(par.Phi), -MathF.Sin(par.Phi) },
					{ 0, MathF.Sin(par.Phi), MathF.Cos(par.Phi) }
				}
			).Transpose();
			var yRotate = Matrix<float>.Build.DenseOfArray(new[,]
				{
					{ MathF.Cos(par.Theta), 0, MathF.Sin(par.Theta) },
					{ 0, 1, 0 },
					{ -MathF.Sin(par.Theta), 0, MathF.Cos(par.Theta) }
				}
			).Transpose();
			var zRotate = Matrix<float>.Build.DenseOfArray(new[,]
			{
				{ MathF.Cos(par.Zenit), -MathF.Sin(par.Zenit), 0 },
				{ MathF.Sin(par.Zenit), MathF.Cos(par.Zenit), 0 },
				{ 0, 0, 1 }
			}).Transpose();

			var backRotateMatrix = zRotate * yRotate * xRotate;
			par.BackRotateMatrix = backRotateMatrix;
		}
		else
		{
			// Matrix was reused
			info?.IncrementBackRotateMatrixReused();
			particleInfo?.IncrementBackRotateMatrixReused();
		}

		foreach (var edge in parallelepipedCoordinates.ForAll())
		{
			DoBackVectorRotate(edge, par.BackRotateMatrix);
		}
	}

	public static Vector<float> DoBackVectorRotate(Vector<float> vec, float Fi, float Theta, float Zenit, Matrix<float>? matrix = null)
	{
		if (matrix is not null)
		{
			return matrix * vec;
		}
		var xRotate = Matrix<float>.Build.DenseOfArray(new[,]
			{
				{ 1, 0, 0 },
				{ 0, MathF.Cos(Fi), -MathF.Sin(Fi) },
				{ 0, MathF.Sin(Fi), MathF.Cos(Fi) }
			}
		).Transpose();
		var yRotate = Matrix<float>.Build.DenseOfArray(new[,]
			{
				{ MathF.Cos(Theta), 0, MathF.Sin(Theta) },
				{ 0, 1, 0 },
				{ -MathF.Sin(Theta), 0, MathF.Cos(Theta) }
			}
		).Transpose();
		var zRotate = Matrix<float>.Build.DenseOfArray(new[,]
		{
			{ MathF.Cos(Zenit), -MathF.Sin(Zenit), 0 },
			{ MathF.Sin(Zenit), MathF.Cos(Zenit), 0 },
			{ 0, 0, 1 }
		}).Transpose();
		
		var res = zRotate * yRotate * xRotate * vec;
		return res;
	}
	
	public static Vector<float> DoBackVectorRotate(Vector<float> vec,
		Matrix<float> backRotateMatrix)
	{
		vec = backRotateMatrix * vec;
		return vec;
	}

	public static void DoParallelepipedTransform(List<Vector<float>> edges, float x, float y,
		float z)
	{
		for (int i = 0; i < edges.Count; i++)
		{
			edges[i] = 	DoVectorTransform(edges[i], x, y, z);
		}
	}
	
	public static void DoParallelepipedTransform(ref ParallelepipedCoordinates parallelepipedCoordinates, float x, float y,
		float z)
	{
		parallelepipedCoordinates.A = DoVectorTransform(parallelepipedCoordinates.A, x, y, z);
		parallelepipedCoordinates.B = DoVectorTransform(parallelepipedCoordinates.B, x, y, z);
		parallelepipedCoordinates.C = DoVectorTransform(parallelepipedCoordinates.C, x, y, z);
		parallelepipedCoordinates.D = DoVectorTransform(parallelepipedCoordinates.D, x, y, z);
		parallelepipedCoordinates.A1 = DoVectorTransform(parallelepipedCoordinates.A1, x, y, z);
		parallelepipedCoordinates.B1 = DoVectorTransform(parallelepipedCoordinates.B1, x, y, z);
		parallelepipedCoordinates.C1 = DoVectorTransform(parallelepipedCoordinates.C1, x, y, z);
		parallelepipedCoordinates.D1 = DoVectorTransform(parallelepipedCoordinates.D1, x, y, z);
	}

	public static Vector<float> DoVectorTransform(Vector<float> vec, float x, float y, float z)
	{
		vec[0] += x;
		vec[1] += y;
		vec[2] += z;
		return vec;
	}
}