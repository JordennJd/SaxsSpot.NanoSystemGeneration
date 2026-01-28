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
        var rawA = gamma.Sample();
                
        return new Sphere((float)rawA, 0, 0, 0);	
    }
}