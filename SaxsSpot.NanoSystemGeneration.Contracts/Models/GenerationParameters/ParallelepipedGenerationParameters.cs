using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

public record ParallelepipedGenerationParameters(
    float Epsilon,
    int Count,
    double? NumericalConcentration,
    double? GlobalSize,
    float MinSize,
    float MaxSize,
    float Theta,
    float K,
    double Excess,
    bool DisableIntersectionOptimizations = false)
    : ParticleGenerationParameters(Count, NumericalConcentration, GlobalSize, MinSize, MaxSize, Theta, K, Excess)
{
    public override ParticleKind GetParticleKind()
    {
        return ParticleKind.Parallelepiped;
    }
}