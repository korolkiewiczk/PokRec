namespace Common;

public record PlayerStatsRelative(
    int Hands,
    double VPIP,
    double PFR,
    double ThreeBet,
    double FoldToThreeBet,
    double CBetFlop,
    double FoldToCBetFlop,
    double WTSD
)
{
    public override string ToString()
    {
        return $"HANDS: {Hands}, VPIP: {VPIP:F1}%, PFR: {PFR:F1}%, 3-Bet: {ThreeBet:F1}%, " +
               $"Fold to 3-Bet: {FoldToThreeBet:F1}%, C-Bet Flop: {CBetFlop:F1}%, " +
               $"Fold to C-Bet: {FoldToCBetFlop:F1}%, WTSD: {WTSD:F1}%";
    }
}