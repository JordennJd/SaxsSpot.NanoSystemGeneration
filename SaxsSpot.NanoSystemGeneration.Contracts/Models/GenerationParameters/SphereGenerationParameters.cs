using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

public record SphereGenerationParameters(
    int Count,
    double? NumericalConcentration,
    double? GlobalSize,
    float MinSize,
    float MaxSize,
    float Theta,
    float K,
    double Excess)
    : ParticleGenerationParameters(Count, NumericalConcentration, GlobalSize, MinSize, MaxSize, Theta, K, Excess)
{
    public override ParticleKind GetParticleKind()
    {
        return ParticleKind.Sphere;
    }
}