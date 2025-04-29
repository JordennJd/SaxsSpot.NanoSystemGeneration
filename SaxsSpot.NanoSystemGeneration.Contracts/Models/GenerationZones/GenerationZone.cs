using System.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;

public record GenerationZone(float GlobalSize, GenerationZoneForm GenerationZoneForm)
{
    public float GetVolume()
    {
        return GenerationZoneForm switch
        {
            GenerationZoneForm.Cube => GlobalSize * GlobalSize * GlobalSize,
            GenerationZoneForm.Sphere => (4f / 3f) * MathF.PI * GlobalSize * GlobalSize * GlobalSize,
            _ => throw new InvalidOperationException("Not supported generation zone form.")
        };
    }
}