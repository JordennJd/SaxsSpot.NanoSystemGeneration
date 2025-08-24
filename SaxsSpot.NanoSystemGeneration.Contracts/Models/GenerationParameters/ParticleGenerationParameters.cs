using Newtonsoft.Json;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

// [JsonConverter(typeof(ParticleConverter))]
public abstract record ParticleGenerationParameters(
    int Count,
    double? NumericalConcentration,
    double? GlobalSize,
    float MinSize,
    float MaxSize,
    float Theta,
    float K,
    double Excess)
{
    public double? GlobalSize { get; private set; } = GlobalSize;
    
    public double? NumericalConcentration { get; private set; } = NumericalConcentration;
    
    public double GetNumericalConcentration(float sumOfVolumes)
    {
        if(NumericalConcentration is null)
        {
            NumericalConcentration = sumOfVolumes / Math.Pow(GlobalSize!.Value, 3);
        }
			
        return NumericalConcentration!.Value;
    }
    public double GetSize(float sumOfVolumes)
    {
        if (GlobalSize is null)
        {
            GlobalSize =  Math.Pow(sumOfVolumes / NumericalConcentration!.Value, 1.0 / 3.0);
        }
		
        return GlobalSize.Value;
    }
    
    public abstract ParticleKind GetParticleKind();
}