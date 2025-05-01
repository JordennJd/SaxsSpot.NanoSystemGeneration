using System.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;

internal static class SAT
{
    public static bool IsIntersect(this Parallelepiped cube1, Parallelepiped cube2)
    {
        Vector3[] vertices1 = cube1.GetVertices();
        Vector3[] vertices2 = cube2.GetVertices();

        Vector3[] axes = GetSeparatingAxes(vertices1, vertices2);

        foreach (var axis in axes)
        {
            if (!IsProjectionOverlap(vertices1, vertices2, axis))
            {
                return false;
            }
        }

        return true;
    }

    private static Vector3[] GetSeparatingAxes(Vector3[] vertices1, Vector3[] vertices2)
    {
        Vector3[] axes = new Vector3[15];
        int index = 0;

        axes[index++] = Vector3.Normalize(vertices1[1] - vertices1[0]);
        axes[index++] = Vector3.Normalize(vertices1[3] - vertices1[0]);
        axes[index++] = Vector3.Normalize(vertices1[4] - vertices1[0]);

        axes[index++] = Vector3.Normalize(vertices2[1] - vertices2[0]);
        axes[index++] = Vector3.Normalize(vertices2[3] - vertices2[0]);
        axes[index++] = Vector3.Normalize(vertices2[4] - vertices2[0]);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                axes[index++] = Vector3.Normalize(Vector3.Cross(axes[i], axes[j + 3]));
            }
        }

        return axes;
    }

    private static bool IsProjectionOverlap(Vector3[] vertices1, Vector3[] vertices2, Vector3 axis)
    {
        (float min1, float max1) = ProjectVertices(vertices1, axis);
        (float min2, float max2) = ProjectVertices(vertices2, axis);

        return max1 >= min2 && max2 >= min1;
    }

    private static (float, float) ProjectVertices(Vector3[] vertices, Vector3 axis)
    {
        float min = Vector3.Dot(vertices[0], axis);
        float max = min;

        for (int i = 1; i < vertices.Length; i++)
        {
            float projection = Vector3.Dot(vertices[i], axis);
            if (projection < min)
            {
                min = projection;
            }
            if (projection > max)
            {
                max = projection;
            }
        }

        return (min, max);
    }
}