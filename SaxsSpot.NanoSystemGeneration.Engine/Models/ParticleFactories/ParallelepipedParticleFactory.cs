using FluentValidation;
using MathNet.Numerics.Distributions;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Validation;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.ParticleFactories;

public class ParallelepipedParticleFactory : ParticleFactory
{
    private readonly Random _random = new();
    
    public override List<Particle> GenerateSystem(ParticleGenerationParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var parallelepipedGenerationParameters = parameters as ParallelepipedGenerationParameters;
        
        var validator = new ParallelepipedGenerationParametersValidator();
        validator!.ValidateAndThrow(parallelepipedGenerationParameters);
        
        var list = new List<Particle>();
		
        var gamma = new Gamma(parallelepipedGenerationParameters!.K, parallelepipedGenerationParameters.Theta);
		
        for(var i = 0; i< parameters.Count; i++)
        {
            list.Add(GenerateSingleParallelepiped(parallelepipedGenerationParameters.MinSize,
                parallelepipedGenerationParameters.MaxSize, gamma, parallelepipedGenerationParameters!.Epsilon));
        }
		
        return list;
    }
    
    private Parallelepiped GenerateSingleParallelepiped(float min, float max, Gamma gamma, float? epsilon)
    {
        switch (max.CompareTo(max))
        {
            case < 0:
                throw new ArgumentException("max - min < 0");
            
            case 0:
                epsilon ??= _random.NextSingle() + 1;
                return new Parallelepiped(min, epsilon.Value);		
            
            default:
                var rawA = (float)gamma.Sample();
        
                epsilon ??= _random.NextSingle() + 1;

                if (!(Math.Abs(min - max) > 0.000001)) 
                    return new Parallelepiped(rawA, epsilon.Value);
                
                var scaledA = min + (rawA / (rawA + 1f)) * (max - min);
                return new Parallelepiped(scaledA, epsilon.Value);
        }
    }
}