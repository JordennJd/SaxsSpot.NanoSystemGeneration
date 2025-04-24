using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Services;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;
using SaxsSpot.NanoSystemGeneration.Engine.Models.ParticleFactories;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public class NanoSystemGenerator : INanoSystemGenerator
{
	private readonly ParticleGenerationParameters _generationParameters;
	private IList<Particle> _particles;
	
	public NanoSystemGenerator(ParticleGenerationParameters generationParameters)
	{
		_generationParameters = generationParameters;
	}
	
	public Task<List<Particle>> GenerateSystem()
	{
        ArgumentNullException.ThrowIfNull(_generationParameters);

        var particles = ParticleFactory.GetSystem(_generationParameters);
        _particles = particles;
        
		return Task.FromResult(particles);
    }

	public async Task DistributeParticles(IProgress<float> progress, CancellationToken cancellationToken)
	{
		var generationZone = await GetGenerationZone();
		
		var cellManager = new CellManager<InMemoryCell>(_particles, generationZone.GlobalSize);

		_particles = _particles
			.OrderBy(p => p.GetVolume())
			.Reverse()
			.ToList();

		try
		{
			var count = 0;

			foreach (var particle in _particles)
			{
				progress?.Report((float)count / _particles.Count * 100f);

				cancellationToken.ThrowIfCancellationRequested();
				for (var i = 0; i < 100000; i++)
				{
					ParticleManipulator.ChangePosition(particle, generationZone.GlobalSize);

					while (!IntersectionService.IsParticleInsideZone(particle, generationZone))
					{
						ParticleManipulator.ChangePosition(particle, generationZone.GlobalSize);
						i++;
					}

					var isAdded = cellManager.TryAddParallelepipedToCell(particle);

					if (isAdded)
					{
						count++;
						break;
					}
				}

			}
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw;
		}
		finally
		{
			await cellManager.ClearAsync();
		}
	}

	public Task<GenerationZone> GetGenerationZone()
	{
		ArgumentNullException.ThrowIfNull(_generationParameters);

		var volumeSum = _particles.Sum(x => x.GetVolume());
		
		//TODO логика обработки excess

		if (_generationParameters.NumericalConcentration is not null and not 0)
		{
			var size = MathF.Pow(volumeSum / _generationParameters.NumericalConcentration.Value, 1f/3f);
			return Task.FromResult(new GenerationZone(size, GenerationZoneForm.Cube));
		}
		
		return Task.FromResult(new GenerationZone(_generationParameters.GlobalSize!.Value, GenerationZoneForm.Cube));
	}
}