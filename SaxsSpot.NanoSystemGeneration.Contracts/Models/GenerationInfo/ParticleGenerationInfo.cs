namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationInfo;

/// <summary>
/// Information about generation metrics for a single particle
/// </summary>
public class ParticleGenerationInfo
{
    /// <summary>
    /// Particle index in the generation sequence
    /// </summary>
    public int ParticleIndex { get; set; }
    
    /// <summary>
    /// Total insert particle attempts for this particle
    /// </summary>
    public int TotalAttempts { get; set; }
    
    /// <summary>
    /// Positive insert particle attempts for this particle
    /// </summary>
    public int PositiveAttempts { get; set; }
    
    /// <summary>
    /// Total change position attempts for this particle
    /// </summary>
    public int TotalChangePositionAttempts { get; set; }
    
    /// <summary>
    /// Count attempts where intersection was found in the first node for this particle
    /// </summary>
    public int FirstNodeIntersectionFindTimes { get; set; }
    
    /// <summary>
    /// Total count of neighbor nodes checked during intersection detection for this particle
    /// </summary>
    public int TotalNeighborsNodesCheckedCount { get; set; }
    
    /// <summary>
    /// Count of IsInterCenterDistanceMoreThenDiagonalCheck works positive for this particle
    /// </summary>
    public int IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive { get; set; }
    
    /// <summary>
    /// Total count of IsInterCenterDistanceMoreThenDiagonalCheck executions for this particle
    /// </summary>
    public int IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal { get; set; }
    
    /// <summary>
    /// Count of IsInterCenterDistanceLessThenSidesCheck works positive for this particle
    /// </summary>
    public int IsInterCenterDistanceLessThenSidesCheckTimesPositive { get; set; }
    
    /// <summary>
    /// Total count of IsInterCenterDistanceLessThenSidesCheck executions for this particle
    /// </summary>
    public int IsInterCenterDistanceLessThenSidesCheckTimesTotal { get; set; }
    
    /// <summary>
    /// Count of ElementaryIntersectCheckOnlyBorders works positive with newPar transformation for this particle
    /// </summary>
    public int ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive { get; set; }
    
    /// <summary>
    /// Total count of ElementaryIntersectCheckOnlyBorders executions with newPar transformation for this particle
    /// </summary>
    public int ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal { get; set; }
    
    /// <summary>
    /// Count of ElementaryIntersectCheckOnlyBorders works positive with oldPar like first argument for this particle
    /// </summary>
    public int ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive { get; set; }
    
    /// <summary>
    /// Total count of ElementaryIntersectCheckOnlyBorders executions with oldPar transformation for this particle
    /// </summary>
    public int ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal { get; set; }
    
    /// <summary>
    /// Total count when back rotate matrix was reused for this particle
    /// </summary>
    public int BackRotateMatrixReused { get; set; }
    
    /// <summary>
    /// Count of SAT intersection checks that found intersection for this particle
    /// </summary>
    public int SATCheckTimesPositive { get; set; }
    
    /// <summary>
    /// Total count of SAT intersection checks performed for this particle
    /// </summary>
    public int SATCheckTimesTotal { get; set; }
    
    /// <summary>
    /// Time spent generating this particle
    /// </summary>
    public TimeSpan GenerationTime { get; set; }
    
    /// <summary>
    /// Volume of the particle
    /// </summary>
    public double Volume { get; set; }
    
    /// <summary>
    /// Diameter of the particle
    /// </summary>
    public float Diameter { get; set; }
    
    /// <summary>
    /// Total count of particles checked for intersection during insertion attempts
    /// </summary>
    public int ParticlesCheckedForIntersection { get; set; }
    
    /// <summary>
    /// Count of attempts where particle was outside the generation zone
    /// </summary>
    public int OutOfZoneAttempts { get; set; }
    
    // Increment methods
    public void IncrementBackRotateMatrixReused() => BackRotateMatrixReused++;
    public void IncrementTotalAttempts() => TotalAttempts++;
    public void IncrementPositiveAttempts() => PositiveAttempts++;
    public void IncrementTotalChangePositionAttempts() => TotalChangePositionAttempts++;
    public void IncrementFirstNodeIntersectionFindTimes() => FirstNodeIntersectionFindTimes++;
    public void IncrementTotalNeighborsNodesCheckedCount() => TotalNeighborsNodesCheckedCount++;
    public void IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive() => IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive++;
    public void IncrementIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal() => IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal++;
    public void IncrementIsInterCenterDistanceLessThenSidesCheckTimesPositive() => IsInterCenterDistanceLessThenSidesCheckTimesPositive++;
    public void IncrementIsInterCenterDistanceLessThenSidesCheckTimesTotal() => IsInterCenterDistanceLessThenSidesCheckTimesTotal++;
    public void IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive() => ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive++;
    public void IncrementElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal() => ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal++;
    public void IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive() => ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive++;
    public void IncrementElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal() => ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal++;
    public void IncrementSATCheckTimesPositive() => SATCheckTimesPositive++;
    public void IncrementSATCheckTimesTotal() => SATCheckTimesTotal++;
    public void IncrementParticlesCheckedForIntersection() => ParticlesCheckedForIntersection++;
    public void IncrementOutOfZoneAttempts() => OutOfZoneAttempts++;
    
    public void SetGenerationTime(TimeSpan value) => GenerationTime = value;
    public void SetVolume(double value) => Volume = value;
    public void SetDiameter(float value) => Diameter = value;
    
    // Calculated properties
    /// <summary>
    /// Calculates the efficiency of particle insertion (ratio of successful attempts to total attempts)
    /// </summary>
    public double GetInsertionEfficiency() => TotalAttempts > 0 ? (double)PositiveAttempts / TotalAttempts : 0;
    
    /// <summary>
    /// Calculates the efficiency of finding intersections in the first node
    /// </summary>
    public double GetFirstNodeIntersectionEfficiency() => TotalAttempts > 0 ? (double)FirstNodeIntersectionFindTimes / TotalAttempts : 0;
    
    /// <summary>
    /// Calculates the average number of neighbor nodes checked per insertion attempt
    /// </summary>
    public double GetAverageNeighborsCheckedPerAttempt() => TotalAttempts > 0 ? (double)TotalNeighborsNodesCheckedCount / TotalAttempts : 0;
    
    /// <summary>
    /// Calculates the efficiency of diagonal distance checks (ratio of positive to total checks)
    /// </summary>
    public double GetDiagonalDistanceCheckEfficiency() => IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal > 0 
        ? (double)IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive / IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal : 0;
    
    /// <summary>
    /// Calculates the efficiency of sides distance checks (ratio of positive to total checks)
    /// </summary>
    public double GetSidesDistanceCheckEfficiency() => IsInterCenterDistanceLessThenSidesCheckTimesTotal > 0 
        ? (double)IsInterCenterDistanceLessThenSidesCheckTimesPositive / IsInterCenterDistanceLessThenSidesCheckTimesTotal : 0;
    
    /// <summary>
    /// Calculates the efficiency of elementary intersection checks with new particle transformation
    /// </summary>
    public double GetElementaryIntersectionNewTransformationEfficiency() => ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal > 0 
        ? (double)ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive / ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal : 0;
    
    /// <summary>
    /// Calculates the efficiency of elementary intersection checks with old particle transformation
    /// </summary>
    public double GetElementaryIntersectionOldTransformationEfficiency() => ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal > 0 
        ? (double)ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive / ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal : 0;
    
    /// <summary>
    /// Calculate BackRotateMatrix reused Efficiency
    /// </summary>
    public double GetBackRotateMatrixReusedEfficiency() => ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal > 0 
        ? (double)BackRotateMatrixReused / ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal : 0;
    
    /// <summary>
    /// Calculates the efficiency of SAT intersection checks (ratio of positive to total checks)
    /// </summary>
    public double GetSATCheckEfficiency() => SATCheckTimesTotal > 0 
        ? (double)SATCheckTimesPositive / SATCheckTimesTotal : 0;
    
    /// <summary>
    /// Calculates the average number of particles checked per insertion attempt
    /// </summary>
    public double GetAverageParticlesCheckedPerAttempt() => TotalAttempts > 0 
        ? (double)ParticlesCheckedForIntersection / TotalAttempts : 0;
    
    /// <summary>
    /// Calculates the ratio of out-of-zone attempts to total attempts
    /// </summary>
    public double GetOutOfZoneAttemptsRatio() => TotalAttempts > 0 
        ? (double)OutOfZoneAttempts / TotalAttempts : 0;
    
    /// <summary>
    /// Calculates attempts that were inside zone (total - out of zone)
    /// </summary>
    public int GetInZoneAttempts() => TotalAttempts - OutOfZoneAttempts;
}
