using SaxsSpot.Core.Contracts.Attributes;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemGeneration.Storage.Contracts;

[SaxsService]
public interface INanoSystemObjectStorage : ICommonObjectStorage<Particle>
{
}