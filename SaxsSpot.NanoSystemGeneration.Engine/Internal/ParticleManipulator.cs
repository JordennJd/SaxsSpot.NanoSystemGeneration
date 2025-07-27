using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

internal static  class ParticleManipulator
{
    /// <summary>
	///     Converts a Parallelepiped object to its corresponding ParallelepipedCoordinates.
	/// </summary>
	/// <param name="par">The Parallelepiped object to convert.</param>
	/// <returns>The ParallelepipedCoordinates object.</returns>
	public static ParallelepipedCoordinates ParallelepipedToParallelepipedCoordinates(Parallelepiped par)
	{
		var A = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 + par.A / 2, 0 + par.A * par.E / 2]);
		var B = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 + par.A / 2, 0 + par.A * par.E / 2]);
		var C = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 + par.A / 2, 0 - par.A * par.E / 2]);
		var D = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 + par.A / 2, 0 - par.A * par.E / 2]);
		var A1 = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 - par.A / 2, 0 + par.A * par.E / 2]);
		var B1 = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 - par.A / 2, 0 + par.A * par.E / 2]);
		var C1 = Vector<float>.Build.DenseOfArray([0 - par.A / 2, 0 - par.A / 2, 0 - par.A * par.E / 2]);
		var D1 = Vector<float>.Build.DenseOfArray([0 + par.A / 2, 0 - par.A / 2, 0 - par.A * par.E / 2]);

		var parCord = new ParallelepipedCoordinates(A, B, C, D, A1, B1, C1, D1);
		DoParallelepipedRotate(ref parCord, par.Phi, par.Theta, par.Zenit);
		DoParallelepipedTransform(ref parCord, par.X, par.Y, par.Z);

		return parCord;
	}

	/// <summary>
	///     Rotates a parallelepiped by the specified angles.
	/// </summary>
	/// <param name="parallelepipedCoordinates">The coordinates of the parallelepiped.</param>
	/// <param name="Fi">The rotation angle around the X-axis.</param>
	/// <param name="Theta">The rotation angle around the Y-axis.</param>
	/// <param name="Zenit">The rotation angle around the Z-axis.</param>
	private static void DoParallelepipedRotate(ref ParallelepipedCoordinates parallelepipedCoordinates, float Fi, float Theta,
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
		
		parallelepipedCoordinates.A = DoVectorRotate(parallelepipedCoordinates.A, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.B = DoVectorRotate(parallelepipedCoordinates.B, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.C = DoVectorRotate(parallelepipedCoordinates.C, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.D = DoVectorRotate(parallelepipedCoordinates.D, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.A1 = DoVectorRotate(parallelepipedCoordinates.A1, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.B1 = DoVectorRotate(parallelepipedCoordinates.B1, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.C1 = DoVectorRotate(parallelepipedCoordinates.C1, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.D1 = DoVectorRotate(parallelepipedCoordinates.D1, xRotate, yRotate, zRotate);
		
		xRotate.Clear();
		yRotate.Clear();
		zRotate.Clear();
	}

	/// <summary>
	///     Performs a back rotation on the given parallelepiped coordinates.
	/// </summary>
	/// <param name="parallelepipedCoordinates">The coordinates of the parallelepiped.</param>
	/// <param name="Fi">The rotation angle in the xy-plane.</param>
	/// <param name="Theta">The rotation angle in the xz-plane.</param>
	/// <param name="Zenit">The rotation angle in the yz-plane.</param>
	public static void DoBackParallelepipedRotate(ref ParallelepipedCoordinates parallelepipedCoordinates, float Fi,
		float Theta, float Zenit)
	{
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
		parallelepipedCoordinates.A = DoBackVectorRotate(parallelepipedCoordinates.A, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.B = DoBackVectorRotate(parallelepipedCoordinates.B, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.C = DoBackVectorRotate(parallelepipedCoordinates.C, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.D = DoBackVectorRotate(parallelepipedCoordinates.D, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.A1 = DoBackVectorRotate(parallelepipedCoordinates.A1, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.B1 = DoBackVectorRotate(parallelepipedCoordinates.B1, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.C1 = DoBackVectorRotate(parallelepipedCoordinates.C1, xRotate, yRotate, zRotate);
		parallelepipedCoordinates.D1 = DoBackVectorRotate(parallelepipedCoordinates.D1, xRotate, yRotate, zRotate);
		
		xRotate.Clear();
		yRotate.Clear();
		zRotate.Clear();
	}

	public static Vector<float> DoBackVectorRotate(Vector<float> vec, float Fi, float Theta, float Zenit)
	{
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
		xRotate.Clear();
		yRotate.Clear();
		zRotate.Clear();
		return res;
	}

	private static Vector<float> DoVectorRotate(Vector<float> vec,
		Matrix<float> xRotate, Matrix<float> yRotate, Matrix<float> zRotate)
	{
		vec = xRotate * yRotate * zRotate * vec;
		return vec;
	}

	private static Vector<float> DoBackVectorRotate(Vector<float> vec,
		Matrix<float> xRotate, Matrix<float> yRotate, Matrix<float> zRotate)
	{
		vec = zRotate * yRotate * xRotate * vec;
		return vec;
	}

	private static void DoParallelepipedTransform(ref ParallelepipedCoordinates parallelepipedCoordinates, float x, float y,
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

	private static Vector<float> DoVectorTransform(Vector<float> vec, float x, float y, float z)
	{
		vec[0] += x;
		vec[1] += y;
		vec[2] += z;
		return vec;
	}
	
	private static readonly Random random = new();
	private static readonly float[] signs = { 1, -1 };
	
	public static void ChangePosition(Particle particle, in float radius)
	{
		var x = GeneratePositiveRandomDoubleLessThenRadius(radius);
		var y = GeneratePositiveRandomDoubleLessThenRadius(radius);
		var z = GeneratePositiveRandomDoubleLessThenRadius(radius);
		var fi = GeneratePositiveRandomDoubleLessThenRadius(3.14f);
		var theta = GeneratePositiveRandomDoubleLessThenRadius(3.14f);
		var zenit = GeneratePositiveRandomDoubleLessThenRadius(3.14f);
		particle.ChangePosition(x, y, z, fi, theta, zenit);
	}

	private static float GenerateRandomDoubleLessThenRadius(in float radius)
	{
		return signs[random.Next(1)] * random.NextSingle() * radius;
	}
	
	private static float GeneratePositiveRandomDoubleLessThenRadius(in float radius)
	{
		return (random.NextSingle() - 0.5f) * radius;
	}
	
	public static double GetAmplitudeByParallelepiped(this Parallelepiped parallelepiped, Vector<float> qVector)
	{
		var ex = parallelepiped.GetRotateMatrix() * Vector<float>.Build.DenseOfArray([1, 0, 0]);
		var ey = parallelepiped.GetRotateMatrix() * Vector<float>.Build.DenseOfArray([0, 1, 0]);
		var ez = parallelepiped.GetRotateMatrix() * Vector<float>.Build.DenseOfArray([0, 0, 1]);
		var qx = ex.DotProduct(qVector);
		var qy = ey.DotProduct(qVector);
		var qz = ez.DotProduct(qVector);
		var amplitude = 8 * (Math.Sin(parallelepiped.A / 2 * qx) *
		                     Math.Sin(parallelepiped.A * parallelepiped.E / 2 * qy) *
		                     Math.Sin(parallelepiped.A / 2 * qz));
		return amplitude;
	}

	private static Matrix<float> GetRotateMatrix(this Parallelepiped parallelepiped)
	{
		var xRotate = Matrix<float>.Build.DenseOfArray(new[,]
			{
				{ 1, 0, 0 },
				{ 0, MathF.Cos(parallelepiped.Phi), -MathF.Sin(parallelepiped.Phi) },
				{ 0, MathF.Sin(parallelepiped.Phi), MathF.Cos(parallelepiped.Phi) }
			}
		);
		
		var yRotate = Matrix<float>.Build.DenseOfArray(new[,]
			{
				{ MathF.Cos(parallelepiped.Theta), 0, MathF.Sin(parallelepiped.Theta) },
				{ 0, 1, 0 },
				{ -MathF.Sin(parallelepiped.Theta), 0, MathF.Cos(parallelepiped.Theta) }
			}
		);
		
		var zRotate = Matrix<float>.Build.DenseOfArray(new[,]
		{
			{ MathF.Cos(parallelepiped.Zenit), -MathF.Sin(parallelepiped.Zenit), 0 },
			{ MathF.Sin(parallelepiped.Zenit), MathF.Cos(parallelepiped.Zenit), 0 },
			{ 0, 0, 1 }
		});
		
		return zRotate * yRotate * xRotate;
	}
}