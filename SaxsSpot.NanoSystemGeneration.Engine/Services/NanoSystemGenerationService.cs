using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Contracts.Services;
using SaxsSpot.NanoSystemGeneration.Engine.Internal;
using SaxsSpot.NanoSystemGeneration.Engine.Models.Cells;
using SaxsSpot.NanoSystemGeneration.Engine.Models.ParticleFactories;
using SaxsSpot.NanoSystemGeneration.Storage.Contracts;

namespace SaxsSpot.NanoSystemGeneration.Engine.Services;

public class NanoSystemGenerationService : INanoSystemGenerationService
{
	private readonly INanoSystemObjectStorage _nanoSystemObjectStorage;

	public NanoSystemGenerationService(INanoSystemObjectStorage nanoSystemObjectStorage)
	{
		_nanoSystemObjectStorage = nanoSystemObjectStorage;
	}

	public async Task<List<Particle>> Generate(ParticleGenerationParameters? parameters, IProgress<float> progress,
	    CancellationToken cancellationToken)
    {
		var particles = ParticleFactory.GetSystem(parameters);
		
		var sumOfVolumes = particles.Sum(x => x.GetVolume());
		var maxSemiDiagonal = particles.MaxBy(x => x.GetVolume())?.GetDiameter() / 2;
		
		var cellCount = Convert.ToInt32(parameters!.GetSize(sumOfVolumes) / (maxSemiDiagonal * 2)) - 1;
		cellCount = cellCount > 70 ? 70 : cellCount;
		var cellSize = parameters.GetSize(sumOfVolumes) / cellCount;
		var cellManager = new CellManager<InMemoryCell>(cellCount, cellSize);

		particles = particles
			.OrderBy(p => p.GetVolume())
			.Reverse()
			.ToList();

		var count = 0;
		foreach (var particle in particles)
		{
			progress?.Report(((float)count++ / cellCount) * 100f);
			
			cancellationToken.ThrowIfCancellationRequested();					
			for (var i = 0; i < 100000; i++)
			{
				ParticleManipulator.ChangePosition(particle, parameters.GetSize(sumOfVolumes));
				
				while (!particle.IsBoundOfCubeZone(new Parallelepiped(parameters.GetSize(sumOfVolumes), 1)))
				{
					ParticleManipulator.ChangePosition(particle, parameters.GetSize(sumOfVolumes));
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
		var pars = cellManager
		.GetParticles()
		.ToList();
		
		await cellManager.ClearAsync();
		
		return pars;    
    }

    public async Task GenerateAndSave(ParticleGenerationParameters? parameters, IProgress<float> progress, CancellationToken cancellationToken)
    {
	    var result = await Generate(parameters, progress, cancellationToken);

	    await _nanoSystemObjectStorage.Save(result, Guid.NewGuid());
    }
}