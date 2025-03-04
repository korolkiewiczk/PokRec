using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Games.TexasHoldem.Solving;

namespace Game.Games.TexasHoldem.Utils;

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

    public static string ToDebugString(this Dictionary<string, PlayerStats> playersStats)
    {
        return string.Join("\n", playersStats.Select(x =>
            $"{x.Key} = {string.Join("|", ToRelativeStats(x.Value))}"));
    }
}