using System.Numerics;
using System.Runtime.CompilerServices;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;

internal static class SAT
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIntersect(in Parallelepiped cube1, in Parallelepiped cube2)
    {
        // Получаем матрицы вращения один раз
        Matrix4x4 rotation1 = Matrix4x4.CreateFromYawPitchRoll(cube1.Theta, cube1.Phi, cube1.Zenit);
        Matrix4x4 rotation2 = Matrix4x4.CreateFromYawPitchRoll(cube2.Theta, cube2.Phi, cube2.Zenit);
        
        // Локальные оси (предварительно вычисленные константы)
        Vector3 localX = Vector3.UnitX;
        Vector3 localY = Vector3.UnitY; 
        Vector3 localZ = Vector3.UnitZ;

        // Преобразуем локальные оси в мировые
        Vector3 axis1_x = TransformNormal(localX, rotation1);
        Vector3 axis1_y = TransformNormal(localY, rotation1);
        Vector3 axis1_z = TransformNormal(localZ, rotation1);
        
        Vector3 axis2_x = TransformNormal(localX, rotation2);
        Vector3 axis2_y = TransformNormal(localY, rotation2);
        Vector3 axis2_z = TransformNormal(localZ, rotation2);

        // Проверяем 6 основных осей
        if (!OverlapOnAxis(in cube1, in cube2, axis1_x, rotation1, rotation2) ||
            !OverlapOnAxis(in cube1, in cube2, axis1_y, rotation1, rotation2) ||
            !OverlapOnAxis(in cube1, in cube2, axis1_z, rotation1, rotation2) ||
            !OverlapOnAxis(in cube1, in cube2, axis2_x, rotation1, rotation2) ||
            !OverlapOnAxis(in cube1, in cube2, axis2_y, rotation1, rotation2) ||
            !OverlapOnAxis(in cube1, in cube2, axis2_z, rotation1, rotation2))
        {
            return false;
        }

        // Проверяем 9 попарных векторных произведений
        if (!OverlapOnCrossAxis(in cube1, in cube2, axis1_x, axis2_x, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_x, axis2_y, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_x, axis2_z, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_y, axis2_x, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_y, axis2_y, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_y, axis2_z, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_z, axis2_x, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_z, axis2_y, rotation1, rotation2) ||
            !OverlapOnCrossAxis(in cube1, in cube2, axis1_z, axis2_z, rotation1, rotation2))
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool OverlapOnAxis(in Parallelepiped cube1, in Parallelepiped cube2, Vector3 axis, 
        Matrix4x4 rot1, Matrix4x4 rot2)
    {
        // Проекция центров
        Vector3 center1 = new Vector3(cube1.X, cube1.Y, cube1.Z);
        Vector3 center2 = new Vector3(cube2.X, cube2.Y, cube2.Z);
        
        float proj1 = Vector3.Dot(center1, axis);
        float proj2 = Vector3.Dot(center2, axis);
        
        // Полуразмеры вдоль оси (оптимизированное вычисление)
        float halfSize1 = GetHalfSizeOptimized(cube1.A, cube1.E, axis, rot1);
        float halfSize2 = GetHalfSizeOptimized(cube2.A, cube2.E, axis, rot2);
        
        return Math.Abs(proj1 - proj2) <= (halfSize1 + halfSize2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool OverlapOnCrossAxis(in Parallelepiped cube1, in Parallelepiped cube2, 
        Vector3 axis1, Vector3 axis2, Matrix4x4 rot1, Matrix4x4 rot2)
    {
        Vector3 crossAxis = Vector3.Cross(axis1, axis2);
        
        // Пропускаем почти нулевые оси (параллельные грани)
        if (crossAxis.LengthSquared() < 1e-12f) 
            return true;
        
        crossAxis = Vector3.Normalize(crossAxis);
        return OverlapOnAxis(in cube1, in cube2, crossAxis, rot1, rot2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetHalfSizeOptimized(float a, float e, Vector3 axis, Matrix4x4 rotation)
    {
        // Локальные полуразмеры (A/2 по X и Y, A*E/2 по Z)
        Vector3 halfExtents = new Vector3(a * 0.5f, a * 0.5f, a * e * 0.5f);
        
        // Локальные оси в мировых координатах
        Vector3 worldX = TransformNormal(Vector3.UnitX, rotation);
        Vector3 worldY = TransformNormal(Vector3.UnitY, rotation);
        Vector3 worldZ = TransformNormal(Vector3.UnitZ, rotation);
        
        // Проекция полуразмеров на ось (формула из OBB)
        return Math.Abs(Vector3.Dot(worldX, axis)) * halfExtents.X +
               Math.Abs(Vector3.Dot(worldY, axis)) * halfExtents.Y +
               Math.Abs(Vector3.Dot(worldZ, axis)) * halfExtents.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector3 TransformNormal(Vector3 normal, Matrix4x4 matrix)
    {
        // Оптимизированное преобразование нормали (без трансляции)
        return Vector3.Normalize(new Vector3(
            normal.X * matrix.M11 + normal.Y * matrix.M21 + normal.Z * matrix.M31,
            normal.X * matrix.M12 + normal.Y * matrix.M22 + normal.Z * matrix.M32,
            normal.X * matrix.M13 + normal.Y * matrix.M23 + normal.Z * matrix.M33
        ));
    }
}