using System.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;

public record GenerationZone(float GlobalSize, GenerationZoneForm GenerationZoneForm)
{
}