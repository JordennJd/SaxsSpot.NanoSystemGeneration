using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

public static class IntersectCheckExtension
{
    public static bool IsIntersect(this Particle source, Particle particle)
    {
        switch (source)
        {
            case Parallelepiped parallelepiped1 when particle is Parallelepiped parallelepiped2:
                return IntersectionService.IsIntersect(parallelepiped1, parallelepiped2);

            case Sphere sphere1 when particle is Sphere sphere2:
                return IntersectionService.IsSphereIntersect(sphere1, sphere2);
        }

        throw new NotImplementedException("For this particle is not supported");
    }
}