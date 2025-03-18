using Game.Common.Model;

namespace Game.Common.Utils;

public static class PlayerStatsExtensions
{
    public static PlayerStatsRelative ToRelativeStats(this PlayerStats stats)
    {
        if (stats.Hands == 0)
        {
            return new PlayerStatsRelative(0,0, 0, 0, 0, 0, 0, 0);
        }

        return new PlayerStatsRelative(
            stats.Hands,
            VPIP: (double) stats.VPIP / stats.Hands * 100,
            PFR: (double) stats.PFR / stats.Hands * 100,
            ThreeBet: (double) stats.ThreeBet / stats.Hands * 100,
            FoldToThreeBet: (double) stats.FoldToThreeBet / stats.Hands * 100,
            CBetFlop: (double) stats.CBetFlop / stats.Hands * 100,
            FoldToCBetFlop: (double) stats.FoldToCBetFlop / stats.Hands * 100,
            WTSD: (double) stats.WTSD / stats.Hands * 100
        );
    }
    
    /// <summary>
    /// Converts PlayerStatsRelative from database to PlayerStats for in-memory use
    /// </summary>
    public static PlayerStats ToStats(this PlayerStatsRelative relativeStats)
    {
        return new PlayerStats
        {
            Hands = relativeStats.Hands,
            VPIP = (int)(relativeStats.VPIP * relativeStats.Hands / 100),
            PFR = (int)(relativeStats.PFR * relativeStats.Hands / 100),
            ThreeBet = (int)(relativeStats.ThreeBet * relativeStats.Hands / 100),
            FoldToThreeBet = (int)(relativeStats.FoldToThreeBet * relativeStats.Hands / 100),
            CBetFlop = (int)(relativeStats.CBetFlop * relativeStats.Hands / 100),
            FoldToCBetFlop = (int)(relativeStats.FoldToCBetFlop * relativeStats.Hands / 100),
            WTSD = (int)(relativeStats.WTSD * relativeStats.Hands / 100)
        };
    }

    public static string ToDebugString(this Dictionary<string, PlayerStats> playersStats)
    {
        return string.Join("\n", playersStats.Select(x =>
            $"{x.Key} = {string.Join("|", ToRelativeStats(x.Value))}"));
    }
}