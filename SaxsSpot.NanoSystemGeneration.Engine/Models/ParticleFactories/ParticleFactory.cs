using System.Reflection;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemGeneration.Engine.Models.ParticleFactories;

public abstract class ParticleFactory
{
    public abstract List<Particle> GenerateSystem(ParticleGenerationParameters? parameters);

    public static List<Particle> GetSystem(ParticleGenerationParameters? parameters)
    {
        var ourtype = typeof(ParticleFactory);
        var list = Assembly.GetAssembly(ourtype).GetTypes()
            .Where(type => type.IsSubclassOf(ourtype));

        foreach (var item in list)
        {	
            var a = item.Name;
            var b = parameters.GetType().Name.Substring(0, 4);
			
            if(a.StartsWith(b))
            {
                var instance = (ParticleFactory)Activator.CreateInstance(item);
                return instance.GenerateSystem(parameters);
            }
        }

        throw new Exception("Factory not found");
    }
}