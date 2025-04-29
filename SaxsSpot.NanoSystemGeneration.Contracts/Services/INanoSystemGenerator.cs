using SaxsSpot.Core.Contracts.Attributes;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Services;

[SaxsService]
public interface INanoSystemGenerator
{
    /// <summary>
    /// distribute particles with no intersection
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<Particle>> DistributeParticles(IProgress<float> progress, CancellationToken cancellationToken);

    /// <summary>
    /// Generate system by parameters
    /// </summary>
    /// <returns></returns>
    Task<List<Particle>> GenerateSystem();
}