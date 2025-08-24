using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Engine.Extensions;

public static class ParallelepipedExtensions
{
    public static double GetAmplitude(this Parallelepiped parallelepiped)
    {
        var rotateMatrix = parallelepiped.GetRotateMatrix();
        var ex = rotateMatrix * Vector<double>.Build.DenseOfArray([1, 0, 0]);
        var ey = rotateMatrix * Vector<double>.Build.DenseOfArray([0, 1, 0]);
        var ez = rotateMatrix * Vector<double>.Build.DenseOfArray([0, 0, 1]);
        var radiusVector = Vector<double>.Build.DenseOfArray([parallelepiped.X, parallelepiped.Y, parallelepiped.Z]);
        var qx = ex.DotProduct(radiusVector);
        var qy = ey.DotProduct(radiusVector);
        var qz = ez.DotProduct(radiusVector);
        
        var amplitude = 8 * (Math.Sin(parallelepiped.A / 2 * qx) *
                             Math.Sin(parallelepiped.A * parallelepiped.E / 2 * qy) *
                             Math.Sin(parallelepiped.A / 2 * qz));
        return amplitude;
    }

    private static Matrix<double> GetRotateMatrix(this Parallelepiped parallelepiped)
    {
        var xRotate = Matrix<double>.Build.DenseOfArray(new[,]
            {
                { 1, 0, 0 },
                { 0, Math.Cos(parallelepiped.Phi), -Math.Sin(parallelepiped.Phi) },
                { 0, Math.Sin(parallelepiped.Phi), Math.Cos(parallelepiped.Phi) }
            }
        );
		
        var yRotate = Matrix<double>.Build.DenseOfArray(new[,]
            {
                { Math.Cos(parallelepiped.Theta), 0, Math.Sin(parallelepiped.Theta) },
                { 0, 1, 0 },
                { -Math.Sin(parallelepiped.Theta), 0, Math.Cos(parallelepiped.Theta) }
            }
        );
		
        var zRotate = Matrix<double>.Build.DenseOfArray(new[,]
        {
            { Math.Cos(parallelepiped.Zenit), -Math.Sin(parallelepiped.Zenit), 0 },
            { Math.Sin(parallelepiped.Zenit), Math.Cos(parallelepiped.Zenit), 0 },
            { 0, 0, 1 }
        });
		
        return xRotate * yRotate * zRotate;
    }
}