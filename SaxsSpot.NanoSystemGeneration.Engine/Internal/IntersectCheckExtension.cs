using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

public static class IntersectCheckExtension
{

    public static bool IsIntersect(this Sphere source, Sphere particle, bool isNeighbors = false, GenerationInfo? info = null)
    {
        return IntersectionService.IsSphereIntersect(source, particle, info);
    }
    
    public static bool IsIntersect(this Parallelepiped source, Parallelepiped particle, bool isNeighbors = false, GenerationInfo? info = null)
    {
        return IntersectionService.IsIntersect(source, particle, isNeighbors: isNeighbors, info: info);
    }
}