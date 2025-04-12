using FluentValidation;
using MathNet.Numerics.Distributions;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Validation;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.ParticleFactories;

public class SphereParticleFactory : ParticleFactory
{
    public override List<Particle> GenerateSystem(ParticleGenerationParameters? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sphereParameters = parameters as SphereGenerationParameters;
        
        var validator = new SphereGenerationParametersValidator();
        validator!.ValidateAndThrow(sphereParameters);
		
        var list = new List<Particle>();
		
        var gamma = new Gamma(sphereParameters!.K, sphereParameters.Theta);

        for(int i = 0; i< sphereParameters.Count; i++)
        {
            list.Add(GenerateSingleSphere(sphereParameters.MinSize, sphereParameters.MaxSize, gamma));
        }
		
        return list;    
    }
    
    private static Sphere GenerateSingleSphere(float min, float max, Gamma gamma)
    {
        if (max.CompareTo(min) < 0)
        {
            throw new ArgumentException("max - min < 0");
        }

        if (min.CompareTo(max) == 0)
        {
            return new Sphere(min);		
        }

        var rawA = gamma.Sample();
        
        var scaledA = min + (rawA / (rawA + 1)) * (max - min);
        
        return new Sphere((float)scaledA, 0, 0, 0);	
    }
}