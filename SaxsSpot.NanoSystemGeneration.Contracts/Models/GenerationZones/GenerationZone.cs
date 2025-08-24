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
}