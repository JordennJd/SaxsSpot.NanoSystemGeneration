using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

public static class IntersectCheckExtension
{

    public static bool IsIntersect(this Particle source, Particle particle, bool isNeighbors = false, GenerationInfo? info = null)
    {
        if (source.ParticleKind == ParticleKind.Parallelepiped)
            return IntersectionService.IsIntersect((Parallelepiped)source, (Parallelepiped)particle, isNeighbors: isNeighbors, info: info);
        
        if (source.ParticleKind == ParticleKind.Sphere)
            return IntersectionService.IsSphereIntersect(source as Sphere, particle as Sphere, info);
        
        throw new ArgumentException("not implement intersection interface");
    }
}