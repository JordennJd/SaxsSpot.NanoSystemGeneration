using System.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;

public record GenerationZone(double GlobalSize, GenerationZoneForm GenerationZoneForm)
{
    public double GetVolume()
    {
        return GenerationZoneForm switch
        {
            GenerationZoneForm.Cube => GlobalSize * GlobalSize * GlobalSize,
            GenerationZoneForm.Sphere => (4d / 3d) * Math.PI * GlobalSize * GlobalSize * GlobalSize,
            _ => throw new InvalidOperationException("Not supported generation zone form.")
        };
    }
    
    public double GetInnerSphereVolume()
    {
        return GenerationZoneForm switch
        {
            GenerationZoneForm.Cube => (4d / 3d) * Math.PI * (GlobalSize/2) * (GlobalSize/2) * (GlobalSize/2),
            GenerationZoneForm.Sphere => (4d / 3d) * Math.PI * GlobalSize * GlobalSize * GlobalSize,
            _ => throw new InvalidOperationException("Not supported generation zone form.")
        };
    }
    
    public double GetInnerSphereVolumeWithMaxParticleRadius(float maxParticleRadius)
    {
        return GenerationZoneForm switch
        {
            GenerationZoneForm.Cube => (4d / 3d) * Math.PI * (GlobalSize/2 + maxParticleRadius) * (GlobalSize/2 + maxParticleRadius) * (GlobalSize/2 + maxParticleRadius),
            GenerationZoneForm.Sphere => (4d / 3d) * Math.PI * (GlobalSize  + maxParticleRadius) * (GlobalSize + maxParticleRadius) * (GlobalSize + maxParticleRadius),
            _ => throw new InvalidOperationException("Not supported generation zone form.")
        };
    }
}