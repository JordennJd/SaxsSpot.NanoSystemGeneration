using System.Globalization;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;

public record ZoneConcentrationAnalyze(int ZoneIndex, double Concentration)
{
    public static ZoneConcentrationAnalyze FromString(string rad)
    {
        var parameters = rad.Split().Select(x => float.Parse(x, NumberFormatInfo.InvariantInfo)).ToList();

        return new ZoneConcentrationAnalyze(float.ConvertToInteger<int>(parameters[0]), parameters[1]);
    }
    
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, 
            "{0} {1}", ZoneIndex, Concentration);
    }
}