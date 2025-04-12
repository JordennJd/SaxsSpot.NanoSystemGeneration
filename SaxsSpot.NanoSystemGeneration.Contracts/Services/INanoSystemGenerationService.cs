using SaxsSpot.Core.Contracts.Attributes;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Services;

[SaxsService]
public interface INanoSystemGenerationService
{
    Task<List<Particle>> Generate(ParticleGenerationParameters? parameters,
        IProgress<float> progress, CancellationToken cancellationToken);
    
    Task GenerateAndSave(ParticleGenerationParameters? parameters,
        IProgress<float> progress, CancellationToken cancellationToken);
}