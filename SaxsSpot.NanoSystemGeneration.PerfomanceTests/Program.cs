using BenchmarkDotNet.Running;

namespace SaxsSpot.NanoSystemGeneration.PerfomanceTests;

public class Program
{
    public static async Task Main(string[] args)
    {
        // BenchmarkRunner.Run<NanoSystemGenerationBenchmark>();
        var a = new NanoSystemGenerationBenchmark();
        a.Setup();
        await a.GenerateAndDistribute();
    }
}