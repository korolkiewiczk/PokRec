namespace Game.Games.TexasHoldem.Solving.AdaptivePokerStrategy;

/// <summary>
/// Configuration class for opponent adjustment superparameters.
/// These weights allow fine tuning of how each opponent statistic influences our strategy.
/// </summary>
public record OpponentAdjustmentConfig
{
    public double VPIPWeight { get; set; } = 1.0;
    public double PFRWeight { get; set; } = 1.0;
    public double ThreeBetWeight { get; set; } = 1.0;
    public double FoldToThreeBetWeight { get; set; } = 1.0;
    public double CBetFlopWeight { get; set; } = 1.0;
    public double FoldToCBetFlopWeight { get; set; } = 1.0;
    public double WTSDWeight { get; set; } = 1.0;
}