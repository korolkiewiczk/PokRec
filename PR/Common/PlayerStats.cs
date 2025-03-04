namespace Common;

public record PlayerStats(
    int Hands = 0,
    int VPIP = 0,
    int PFR = 0,
    int ThreeBet = 0,
    int FoldToThreeBet = 0,
    int CBetFlop = 0,
    int FoldToCBetFlop = 0,
    int WTSD = 0
);