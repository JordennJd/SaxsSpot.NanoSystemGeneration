using Newtonsoft.Json;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

// [JsonConverter(typeof(ParticleConverter))]
public abstract record ParticleGenerationParameters(
    int Count,
    float? NumericalConcentration,
    float? GlobalSize,
    float MinSize,
    float MaxSize,
    float Theta,
    float K,
    float Excess)
{
    public float? GlobalSize { get; private set; } = GlobalSize;
    
    public float? NumericalConcentration { get; private set; } = NumericalConcentration;
    
    public float GetNumericalConcentration(float sumOfVolumes)
    {
        if(NumericalConcentration is null)
        {
            NumericalConcentration = sumOfVolumes / MathF.Pow(GlobalSize!.Value, 3);
        }
			
        return NumericalConcentration!.Value;
    }
    public float GetSize(float sumOfVolumes)
    {
        if (GlobalSize is null)
        {
            GlobalSize =  MathF.Pow(sumOfVolumes / NumericalConcentration!.Value, 1.0f / 3.0f);
        }
		
        return GlobalSize.Value;
    }
    
    public abstract ParticleKind GetParticleKind();
}