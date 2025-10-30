using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using SaxsSpot.NanoSystemGeneration.Engine.Models.ParticleFactories;
using SaxsSpot.NanoSystemGeneration.Engine.Models.QuadTree;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public class NanoSystemGenerator
{
	private readonly ParticleGenerationParameters _generationParameters;
	private IList<Particle>? _particles;
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
		ArgumentNullException.ThrowIfNull(_generationParameters);
		ArgumentNullException.ThrowIfNull(_particles);
		_generationZone ??= await GetGenerationZone();

		// var generationZone = await GetGenerationZone();
		
		// var cellManager = new CellManager<InMemoryCell>(_particles, generationZone.GlobalSize);
		var tree = new TernaryTreeNode(
			_particles!.MaxBy(x => x.GetDiameter())!.GetDiameter() * 2, _generationZone.GlobalSize);
		
		_particles = _particles
			.OrderBy(p => p.GetVolume())
			.Reverse()
			.ToList();
		var info = new GenerationInfo();
		try
		{
			var handledParticles = 0;
			var attempCount = 100000;
			foreach (var particle in _particles)
			{
				progress?.Report((float)handledParticles / _particles.Count * 100f);
				cancellationToken.ThrowIfCancellationRequested();

				for (var i = 0; i < attempCount; i++)
				{
					ParticleManipulator.ChangePosition(particle, _generationZone.GlobalSize);
 
					if (!IntersectionService.IsParticleInsideZone(particle, _generationZone))
					{
						continue;
					}

					var isAdded = tree.TryInsertParticle(particle, info);

					if (isAdded)
					{
						break;
					}
				}
				
				handledParticles++;
			}
			


			if (_excessedGenerationZone is not null)
			{
				return tree
					.GetParticles()
					.Where(particle => IntersectionService.IsParticleInsideZone(particle, _excessedGenerationZone))
					.ToList();
			}

			return tree
				.GetParticles()
				.ToList();
			
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
			//await cellManager.ClearAsync();
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
				(int)(_generationParameters.Excess * _generationParameters.Count * 6.0 * MathF.Pow(1 + c, 3) /
				      MathF.PI);
			
			var newExcessedSystem = ParticleFactory.GetSystem(_generationParameters with { Count = particleCountInCubeZone });
			_particles = newExcessedSystem;
			
			var globalCubeSize =
				Math.Pow(newExcessedSystem.Sum(x => x.GetVolume())
				          / (_generationParameters.Excess * _generationParameters.NumericalConcentration!.Value),
					1.0d / 3.0d);
			
			_excessedGenerationZone = new GenerationZone(globalCubeSize / (2.0d * (1d + c)), GenerationZoneForm.Sphere);
			_generationZone = new GenerationZone(globalCubeSize, GenerationZoneForm.Cube);
			
			return Task.FromResult(_generationZone);
		}

		if (_generationParameters.NumericalConcentration is not null and not 0)
		{
			var size = Math.Pow(volumeSum / _generationParameters.NumericalConcentration.Value, 1d/3d);
			_generationZone = new GenerationZone(size, GenerationZoneForm.Cube);
			return Task.FromResult(_generationZone);
		}
		
		_generationZone = new GenerationZone(_generationParameters.GlobalSize!.Value, GenerationZoneForm.Cube);
		return Task.FromResult(_generationZone);
	}
}