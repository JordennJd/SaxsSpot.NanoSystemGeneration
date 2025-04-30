using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
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
	private GenerationZone? _generationZone;
	private GenerationZone? _excessedGenerationZone;
	
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

	public async Task<IList<Particle>> DistributeParticles(IProgress<float> progress, CancellationToken cancellationToken)
	{
		var generationZone = await GetGenerationZone();
		
		var cellManager = new CellManager<InMemoryCell>(_particles, generationZone.GlobalSize);

		_particles = _particles
			.OrderBy(p => p.GetVolume())
			.Reverse()
			.ToList();

		try
		{
			var handledParticles = 0;

			foreach (var particle in _particles)
			{
				progress?.Report((float)handledParticles / _particles.Count * 100f);

				cancellationToken.ThrowIfCancellationRequested();
				for (var i = 0; i < 1000000; i++)
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
						break;
					}
				}
				
				handledParticles++;
			}

			if (_excessedGenerationZone is not null)
			{
				return cellManager
					.GetParticles()
					.Where(particle => IntersectionService.IsParticleInsideZone(particle, _excessedGenerationZone))
					.ToList();
			}

			return cellManager
				.GetParticles();
			
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
		if (_excessedGenerationZone is not null && _generationParameters.Excess != 0)
		{
			return Task.FromResult(_excessedGenerationZone);
		}	
		
		if (_generationZone is not null) return Task.FromResult(_generationZone);
		
		ArgumentNullException.ThrowIfNull(_generationParameters);

		var volumeSum = _particles.Sum(x => x.GetVolume());

		var c = 0.1f;
		
		//TODO логика обработки excess
		if (_generationParameters.Excess != 0)
		{
			var particleCountInCubeZone = 
				_generationParameters.GetParticleKind() is ParticleKind.Sphere ? 
				(int)(_generationParameters.Excess * _generationParameters.Count * 6.0 * MathF.Pow(1 + c, 3) / MathF.PI)
				: (int)(_generationParameters.Excess * _generationParameters.Count * MathF.Pow(1 + c, 3));
			
			var newExcessedSystem = ParticleFactory.GetSystem(_generationParameters with { Count = particleCountInCubeZone });
			_particles = newExcessedSystem;
			
			var globalCubeSize =
				MathF.Pow(newExcessedSystem.Sum(x => x.GetVolume()) 
				          / (_generationParameters.Excess * _generationParameters.NumericalConcentration!.Value),
					1.0f / 3.0f);
			
			_excessedGenerationZone = new GenerationZone(globalCubeSize / (2.0f * (1f + c)), GenerationZoneForm.Sphere);
			_generationZone = new GenerationZone(globalCubeSize, GenerationZoneForm.Cube);
			
			return Task.FromResult(_generationZone);
		}

		if (_generationParameters.NumericalConcentration is not null and not 0)
		{
			var size = MathF.Pow(volumeSum / _generationParameters.NumericalConcentration.Value, 1f/3f);
			_generationZone = new GenerationZone(size, GenerationZoneForm.Cube);
			return Task.FromResult(_generationZone);
		}
		
		_generationZone = new GenerationZone(_generationParameters.GlobalSize!.Value, GenerationZoneForm.Cube);
		return Task.FromResult(_generationZone);
	}
}