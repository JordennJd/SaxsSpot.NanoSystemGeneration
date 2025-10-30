public class GenerationInfo
{
    /// <summary>
    /// Total insert particle attempts
    /// </summary>
    internal int TotalAttempts;
    
    /// <summary>
    /// Positive insert particle attempts
    /// </summary>
    internal int PositiveAttempts;
    
    /// <summary>
    /// Total change position attempts
    /// </summary>
    internal int TotalChangePositionAttempts;

    /// <summary>
    /// Time of build the ternary tree
    /// </summary>
    internal TimeSpan TreeBuildTime;
    
    /// <summary>
    /// Time of generation
    /// </summary>
    internal TimeSpan GenerationTime;

    /// <summary>
    /// Count attempts where intersection was found in the first node
    /// </summary>
    internal int FirstNodeIntersectionFindTimes;
    
    /// <summary>
    /// Total count of neighbor nodes checked during intersection detection
    /// </summary>
    internal int TotalNeighborsNodesCheckedCount;

    /// <summary>
    /// Count of IsInterCenterDistanceMoreThenDiagonalCheck works positive
    /// </summary>
    internal int IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive;
    
    /// <summary>
    /// Total count of IsInterCenterDistanceMoreThenDiagonalCheck executions
    /// </summary>
    internal int IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal;
    
    /// <summary>
    /// Count of IsInterCenterDistanceLessThenSidesCheck works positive
    /// </summary>
    internal int IsInterCenterDistanceLessThenSidesCheckTimesPositive;
    
    /// <summary>
    /// Total count of IsInterCenterDistanceLessThenSidesCheck executions
    /// </summary>
    internal int IsInterCenterDistanceLessThenSidesCheckTimesTotal;

    /// <summary>
    /// Count of ElementaryIntersectCheckOnlyBorders works positive with newPar transformation
    /// </summary>
    internal int ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive;
    
    /// <summary>
    /// Total count of ElementaryIntersectCheckOnlyBorders executions with newPar transformation
    /// </summary>
    internal int ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal;
    
    /// <summary>
    /// Count of ElementaryIntersectCheckOnlyBorders works positive with oldPar like first argument
    /// </summary>
    internal int ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive;
    
    /// <summary>
    /// Total count of ElementaryIntersectCheckOnlyBorders executions with oldPar transformation
    /// </summary>
    internal int ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal;
    
    /// <summary>
    /// Total count when back rotate matrix was reused
    /// </summary>
    internal int BackRotateMatrixReused;

    // Setters for all fields
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

    public void SetTreeBuildTime(TimeSpan value) => TreeBuildTime = value;
    public void SetGenerationTime(TimeSpan value) => GenerationTime = value;
    
    // Getters for all fields
    /// <summary>
    /// Gets total number of particle insertion attempts
    /// </summary>
    public int GetTotalAttempts() => TotalAttempts;
    
    // Getters for all fields
    /// <summary>
    /// Gets total number of particle insertion attempts
    /// </summary>
    public int GetBackRotateMatrixReused() => BackRotateMatrixReused;
    
    /// <summary>
    /// Gets number of successful particle insertion attempts
    /// </summary>
    public int GetPositiveAttempts() => PositiveAttempts;
    
    /// <summary>
    /// Gets total number of particle position change attempts
    /// </summary>
    public int GetTotalChangePositionAttempts() => TotalChangePositionAttempts;

    /// <summary>
    /// Gets time spent building the ternary tree
    /// </summary>
    public TimeSpan GetTreeBuildTime() => TreeBuildTime;
    
    /// <summary>
    /// Gets time spent on particle generation
    /// </summary>
    public TimeSpan GetGenerationTime() => GenerationTime;

    /// <summary>
    /// Gets count of attempts where intersection was detected in the first node
    /// </summary>
    public int GetFirstNodeIntersectionFindTimes() => FirstNodeIntersectionFindTimes;
    
    /// <summary>
    /// Gets total count of neighbor nodes checked during intersection detection
    /// </summary>
    public int GetTotalNeighborsNodesCheckedCount() => TotalNeighborsNodesCheckedCount;

    /// <summary>
    /// Gets count of positive diagonal distance checks
    /// </summary>
    public int GetIsInterCenterDistanceMoreThenDiagonalCheckTimesPositive() => IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive;
    
    /// <summary>
    /// Gets total count of diagonal distance checks performed
    /// </summary>
    public int GetIsInterCenterDistanceMoreThenDiagonalCheckTimesTotal() => IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal;
    
    /// <summary>
    /// Gets count of positive sides distance checks
    /// </summary>
    public int GetIsInterCenterDistanceLessThenSidesCheckTimesPositive() => IsInterCenterDistanceLessThenSidesCheckTimesPositive;
    
    /// <summary>
    /// Gets total count of sides distance checks performed
    /// </summary>
    public int GetIsInterCenterDistanceLessThenSidesCheckTimesTotal() => IsInterCenterDistanceLessThenSidesCheckTimesTotal;

    /// <summary>
    /// Gets count of positive elementary intersection checks with new particle transformation
    /// </summary>
    public int GetElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive() => ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive;
    
    /// <summary>
    /// Gets total count of elementary intersection checks with new particle transformation
    /// </summary>
    public int GetElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal() => ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal;
    
    /// <summary>
    /// Gets count of positive elementary intersection checks with old particle transformation
    /// </summary>
    public int GetElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive() => ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive;
    
    /// <summary>
    /// Gets total count of elementary intersection checks with old particle transformation
    /// </summary>
    public int GetElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal() => ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal;

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
    /// <returns></returns>
    public double GetBackRotateMatrixReusedEfficiency() => ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal > 0 
        ? (double)BackRotateMatrixReused / ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal : 0;

    
    /// <summary>
    /// Calculates the total time spent on tree building and generation
    /// </summary>
    public TimeSpan GetTotalTime() => TreeBuildTime + GenerationTime;
    
    /// <summary>
    /// Calculates the ratio of tree build time to total execution time
    /// </summary>
    public double GetTreeBuildTimeRatio()
    {
        var totalTime = GetTotalTime();
        return totalTime.TotalMilliseconds > 0 ? TreeBuildTime.TotalMilliseconds / totalTime.TotalMilliseconds : 0;
    }
}