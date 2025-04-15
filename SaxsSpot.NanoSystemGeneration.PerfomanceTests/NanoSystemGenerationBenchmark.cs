using BenchmarkDotNet.Attributes;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Services;

[MemoryDiagnoser]
public class NanoSystemGenerationBenchmark
{
    private ParallelepipedGenerationParameters generationParameters;
    private NanoSystemGenerator nanoSystemGenerator;

    [GlobalSetup]
    public void Setup()
    {
        generationParameters = new ParallelepipedGenerationParameters(
            1,
            10000,
            0.2f,
            null,
            1f,
            1f,
            3f,
            1f,
            6f
        );

        nanoSystemGenerator = new NanoSystemGenerator(generationParameters);
    }

    [Benchmark]
    public async Task GenerateAndDistribute()
    {
        var system = await nanoSystemGenerator.GenerateSystem();
        await nanoSystemGenerator.DistributeParticles(new Progress<float>(), CancellationToken.None);
    }
}