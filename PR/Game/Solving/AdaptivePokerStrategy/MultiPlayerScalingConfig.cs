namespace Game.Solving.AdaptivePokerStrategy;

/// <summary>
/// Configuration for multi-player strategy adjustments.
/// These parameters control how pairwise and composite strategies are blended
/// and how aggressive actions are scaled based on the number of opponents.
/// </summary>
public record MultiPlayerScalingConfig
{
    /// <summary>
    /// Coefficient used to downscale aggressive actions as the number of opponents increases.
    /// For example, if set to 0.1, then with 3 opponents the aggressive factor becomes
    /// 1 / (1 + (3-1)*0.1) ≈ 0.83.
    /// </summary>
    public float AggressiveScalingCoefficient { get; set; } = 0.1f;

    /// <summary>
    /// Weight for the composite (aggregated opponent) strategy.
    /// </summary>
    public float CompositeWeight { get; set; } = 0.5f;

    /// <summary>
    /// Weight for the average of pairwise strategies.
    /// </summary>
    public float PairwiseWeight { get; set; } = 0.5f;
}