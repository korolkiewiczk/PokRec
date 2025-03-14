using Game.Common.Model;

namespace Game.Datalayer.Model;

public class Stats
{
    public int Id { get; set; }
    public int NicknameId { get; set; }
    public int Hands { get; set; }
    public decimal VPIP { get; set; }
    public decimal PFR { get; set; }
    public decimal ThreeBet { get; set; }
    public decimal FoldToThreeBet { get; set; }
    public decimal CBetFlop { get; set; }
    public decimal FoldToCBetFlop { get; set; }
    public decimal WTSD { get; set; }

    public Nickname Nickname { get; set; } = null!;
    
    public static Stats FromStatsRelative(int nicknameId, PlayerStatsRelative stats)
    {
        return new Stats
        {
            NicknameId = nicknameId,
            Hands = stats.Hands,
            VPIP = (decimal)stats.VPIP,
            PFR = (decimal)stats.PFR,
            ThreeBet = (decimal)stats.ThreeBet,
            FoldToThreeBet = (decimal)stats.FoldToThreeBet,
            CBetFlop = (decimal)stats.CBetFlop,
            FoldToCBetFlop = (decimal)stats.FoldToCBetFlop,
            WTSD = (decimal)stats.WTSD
        };
    }
    
    public PlayerStatsRelative ToStatsRelative()
    {
        return new PlayerStatsRelative(
            Hands,
            (double)VPIP,
            (double)PFR,
            (double)ThreeBet,
            (double)FoldToThreeBet,
            (double)CBetFlop,
            (double)FoldToCBetFlop,
            (double)WTSD
        );
    }
}