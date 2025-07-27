using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Engine.Extensions;

public static class ParallelepipedExtention
{
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