using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

public static class IntersectCheckExtension
{

    public static bool IsIntersect(this Particle source, Particle particle)
    {
        if (source is Parallelepiped)
            return IntersectionService.IsIntersect(particle as Parallelepiped, source as Parallelepiped);
        
        if (source is Sphere)
            return IntersectionService.IsSphereIntersect(source as Sphere, particle as Sphere);
        
        throw new ArgumentException("not implement intersection interface");
    }
}