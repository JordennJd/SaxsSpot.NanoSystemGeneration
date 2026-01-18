using MathNet.Numerics;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using SaxsSpot.NanoSystemGeneration.Engine.Models.ParticleFactories;
using SaxsSpot.NanoSystemGeneration.Engine.Models.TernaryTree;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public class NanoSystemGenerator(ParticleGenerationParameters generationParameters)
{
	private IList<Particle>? _particles;
	private GenerationZone? _generationZone;
	private GenerationZone? _excessedGenerationZone;
	private bool _isDistributed = false;

	public Task<List<Particle>> GenerateSystem()
	{
        ArgumentNullException.ThrowIfNull(generationParameters);

        var particles = ParticleFactory.GetSystem(generationParameters);
        _particles = particles;
        
		return Task.FromResult(particles);
    }

	public async Task<IList<Particle>> DistributeParticles(IProgress<float> progress, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		
		if (_isDistributed)
		{
			throw new InvalidOperationException("particles already distributed");
		}
		ArgumentNullException.ThrowIfNull(generationParameters);
		ArgumentNullException.ThrowIfNull(_particles);
		
		_generationZone ??= await GetGenerationZone();
		
		if (generationParameters.GetParticleKind() == ParticleKind.Sphere)
		{
			var tree = new TernaryTreeNodeSphere(
				_particles!.MaxBy(x => x.GetDiameter())!.GetDiameter() * 2, _generationZone.GlobalSize);
			
			var spheres = _particles
			.OrderByDescending(p => p.GetVolume())
			.Select(x => (Sphere)x)
			.ToList();
			
			var info = new GenerationInfo();
			var globalSizeFloat = (float)_generationZone.GlobalSize;
			try
			{
				var handledParticles = 0;
				var attemptCount = 100000;
				foreach (var particle in spheres)
				{
					cancellationToken.ThrowIfCancellationRequested();
					
					progress?.Report(100f * handledParticles / spheres.Count);
					for (var i = 0; i < attemptCount; i++)
					{						
						ParticleManipulator.ChangePosition(particle, globalSizeFloat);
	 
						if (!IntersectionService.IsParticleInsideCubeZoneSphere(particle, _generationZone))
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
					_isDistributed = true;
					return tree
						.GetParticles()
						.Where(particle => IntersectionService.IsParticleInsideSphereCubeZoneSphere(particle, _excessedGenerationZone))
						.Select(Particle (x) => x)
						.ToList();
				}

				_isDistributed = true;
				return tree
					.GetParticles()
					.Select(Particle (x) => x)
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
		if (generationParameters.GetParticleKind() == ParticleKind.Parallelepiped)
		{
			var tree = new TernaryTreeNodeParallelepiped(
				_particles!.MaxBy(x => x.GetDiameter())!.GetDiameter() * 2, _generationZone.GlobalSize);
			
			var spheres = _particles
				.OrderByDescending(p => p.GetVolume())
				.Select(x => (Parallelepiped)x)
				.ToList();
			
			var info = new GenerationInfo();
			var globalSizeFloat = (float)_generationZone.GlobalSize;
			try
			{
				var handledParticles = 0;
				var attemptCount = 100000;
				foreach (var particle in spheres)
				{
					cancellationToken.ThrowIfCancellationRequested();
					
					progress?.Report(100f * handledParticles / spheres.Count);
					for (var i = 0; i < attemptCount; i++)
					{
						ParticleManipulator.ChangePosition(particle, globalSizeFloat);
	 
						if (!IntersectionService.IsParticleInsideZoneParallelepiped(particle, _generationZone))
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
					_isDistributed = true;
					return tree
						.GetParticles()
						.Where(particle => IntersectionService.IsParallelepipedInBoundsOfSphereZone(_excessedGenerationZone.GlobalSize, particle))
						.Select(Particle (x) => x)
						.ToList();
				}

				_isDistributed = true;
				return tree
					.GetParticles()
					.Select(Particle (x) => x)
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
		throw new NotImplementedException();
	}

	public Task<GenerationZone> GetGenerationZone()
	{
		if (_excessedGenerationZone is not null && generationParameters.Excess != 0)
		{
			return Task.FromResult(_excessedGenerationZone);
		}	
		
		if (_generationZone is not null) return Task.FromResult(_generationZone);
		
		ArgumentNullException.ThrowIfNull(generationParameters);

		var volumeSum = _particles.Sum(x => x.GetVolume());

		var c = 0.001f;
		
		//TODO логика обработки excess
		if (generationParameters.Excess != 0)
		{
			var particleCountInCubeZone =
				(int)(generationParameters.Excess * generationParameters.Count * 6.0 * MathF.Pow(1 + c, 3) /
				      MathF.PI);
			
			var newExcessedSystem = ParticleFactory.GetSystem(generationParameters with { Count = particleCountInCubeZone });
			_particles = newExcessedSystem;
			
			var globalCubeSize =
				Math.Pow(newExcessedSystem.Sum(x => x.GetVolume())
				          / (generationParameters.Excess * generationParameters.NumericalConcentration!.Value),
					1.0d / 3.0d);
			
			_excessedGenerationZone = new GenerationZone(globalCubeSize / (2.0d * (1d + c)), GenerationZoneForm.Sphere);
			_generationZone = new GenerationZone(globalCubeSize, GenerationZoneForm.Cube);
			
			return Task.FromResult(_generationZone);
		}

		if (generationParameters.NumericalConcentration is not null and not 0)
		{
			var size = Math.Pow(volumeSum / generationParameters.NumericalConcentration.Value, 1d/3d);
			_generationZone = new GenerationZone(size, GenerationZoneForm.Cube);
			return Task.FromResult(_generationZone);
		}
		
		_generationZone = new GenerationZone(generationParameters.GlobalSize!.Value, GenerationZoneForm.Cube);
		return Task.FromResult(_generationZone);
	}
}