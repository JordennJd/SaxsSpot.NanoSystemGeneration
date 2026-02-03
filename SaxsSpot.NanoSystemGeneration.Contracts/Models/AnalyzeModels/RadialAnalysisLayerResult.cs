namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;

/// <summary>
/// Result of radial analysis for a single layer (zone).
/// </summary>
/// <param name="ZoneIndex">Index of the layer/zone.</param>
/// <param name="LayerFrom">Inner radius (layer start).</param>
/// <param name="LayerTo">Outer radius (layer end).</param>
/// <param name="NumericalConcentration">Concentration in the layer.</param>
/// <param name="PointCount">Number of sample points in the layer.</param>
public record RadialAnalysisLayerResult(
    int ZoneIndex,
    double LayerFrom,
    double LayerTo,
    double NumericalConcentration,
    int PointCount);
