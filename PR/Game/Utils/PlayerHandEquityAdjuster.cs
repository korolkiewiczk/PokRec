using System;
using System.Collections.Generic;
using System.Linq;
using Game.Common.Model;

public static class PlayerHandEquityAdjuster
{
    // Constants for stat baselines and adjustment factors
    private const double VPIP_BASELINE = 0.3;
    private const double VPIP_ADJUSTMENT_FACTOR = 0.05;

    private const double PFR_BASELINE = 0.2;
    private const double PFR_ADJUSTMENT_FACTOR = 0.05;

    private const double THREE_BET_FACTOR = 0.03;
    private const double FOLD_TO_THREE_BET_FACTOR = 0.03;

    private const double CBET_FLOP_FACTOR = 0.02;
    private const double FOLD_TO_CBET_FLOP_FACTOR = 0.02;

    private const double WTSD_FACTOR = 0.04;

    // Constants for weighting based on hands played
    private const int MIN_HANDS_FOR_ADJUSTMENT = 10;
    private const int MAX_HANDS_FOR_WEIGHT = 100; // Reference for convergence target
    private const double HANDS_EXP_GROWTH_RATE = 0.05117; // Derived so that at 100 hands weight ~ 0.99

    /// <summary>
    /// Adjusts a dictionary of base hand equities using the player's statistical profile.
    /// </summary>
    /// <param name="baseEquities">A dictionary mapping hand keys to base equity values</param>
    /// <param name="stats">The player's statistical profile (in percentages, with Hands as count)</param>
    /// <returns>A dictionary with adjusted equity values</returns>
    public static Dictionary<string, double> AdjustHandEquities(Dictionary<string, double> baseEquities, PlayerStatsRelative stats)
    {
        var adjustedEquities = new Dictionary<string, double>();

        foreach (var entry in baseEquities)
        {
            adjustedEquities.Add(entry.Key, AdjustEquity(entry.Value, stats));
        }

        return adjustedEquities;
    }
    
    public static PlayerStatsRelative ComputeWeightedAverageStats(List<PlayerStatsRelative> stats)
    {
        double totalWeight = 0;

        double weightedVPIP = 0;
        double weightedPFR = 0;
        double weightedThreeBet = 0;
        double weightedFoldToThreeBet = 0;
        double weightedCBetFlop = 0;
        double weightedFoldToCBetFlop = 0;
        double weightedWTSD = 0;

        foreach (var stat in stats)
        {
            double weight = GetHandsWeight(stat.Hands);
            if (weight == 0)
                continue;

            totalWeight += weight;

            weightedVPIP += stat.VPIP * weight;
            weightedPFR += stat.PFR * weight;
            weightedThreeBet += stat.ThreeBet * weight;
            weightedFoldToThreeBet += stat.FoldToThreeBet * weight;
            weightedCBetFlop += stat.CBetFlop * weight;
            weightedFoldToCBetFlop += stat.FoldToCBetFlop * weight;
            weightedWTSD += stat.WTSD * weight;
        }

        if (totalWeight == 0)
            return new PlayerStatsRelative(0, 0, 0, 0, 0, 0, 0, 0);

        return new PlayerStatsRelative(
            Hands: (int)(stats.Sum(s => s.Hands) / stats.Count), // Optional averaging for total hands
            VPIP: weightedVPIP / totalWeight,
            PFR: weightedPFR / totalWeight,
            ThreeBet: weightedThreeBet / totalWeight,
            FoldToThreeBet: weightedFoldToThreeBet / totalWeight,
            CBetFlop: weightedCBetFlop / totalWeight,
            FoldToCBetFlop: weightedFoldToCBetFlop / totalWeight,
            WTSD: weightedWTSD / totalWeight
        );
    }

    /// <summary>
    /// Adjusts the base equity of a hand based on the player's statistics.
    /// All statistical values are provided as percentages (e.g., 50.0 for 50%).
    /// The modifications are weighted by the number of hands the player has played.
    /// For players with 10 or fewer hands, no adjustment is applied. From 10 to 100 hands,
    /// the weight quickly converges to 1.
    /// </summary>
    /// <param name="baseEquity">The base equity value (from Monte Carlo simulation)</param>
    /// <param name="stats">The player's statistical profile (in percentages, with Hands as count)</param>
    /// <returns>The adjusted equity value</returns>
    private static double AdjustEquity(double baseEquity, PlayerStatsRelative stats)
    {
        // Convert percentage values to fractions (e.g., 50.0 -> 0.50)
        double vpip = stats.VPIP / 100.0;
        double pfr = stats.PFR / 100.0;
        double threeBet = stats.ThreeBet / 100.0;
        double foldToThreeBet = stats.FoldToThreeBet / 100.0;
        double cBetFlop = stats.CBetFlop / 100.0;
        double foldToCBetFlop = stats.FoldToCBetFlop / 100.0;
        double wtsd = stats.WTSD / 100.0;

        // Compute the raw adjustment factor based on the stats.
        double adjustmentFactor = 1.0;

        // Adjust for VPIP (Voluntarily Put Money In Pot)
        if (vpip > VPIP_BASELINE)
        {
            adjustmentFactor += VPIP_ADJUSTMENT_FACTOR * (vpip - VPIP_BASELINE);
        }
        else
        {
            adjustmentFactor -= VPIP_ADJUSTMENT_FACTOR * (VPIP_BASELINE - vpip);
        }

        // Adjust for PFR (Pre-Flop Raise)
        if (pfr > PFR_BASELINE)
        {
            adjustmentFactor += PFR_ADJUSTMENT_FACTOR * (pfr - PFR_BASELINE);
        }
        else
        {
            adjustmentFactor -= PFR_ADJUSTMENT_FACTOR * (PFR_BASELINE - pfr);
        }

        // Adjust for ThreeBet aggression
        adjustmentFactor += THREE_BET_FACTOR * threeBet;

        // Adjust for FoldToThreeBet: Lower fold rate implies the player is more inclined to continue.
        adjustmentFactor -= FOLD_TO_THREE_BET_FACTOR * (1 - foldToThreeBet);

        // Adjust for CBetFlop and FoldToCBetFlop
        adjustmentFactor += CBET_FLOP_FACTOR * cBetFlop;
        adjustmentFactor -= FOLD_TO_CBET_FLOP_FACTOR * (1 - foldToCBetFlop);

        // Adjust for WTSD (Went To Showdown)
        adjustmentFactor += WTSD_FACTOR * wtsd;

        // Weight the adjustment based on the number of hands the player has played.
        double handsWeight = GetHandsWeight(stats.Hands);

        // Blend the adjustment factor with the neutral factor (1) based on the weight.
        double weightedAdjustmentFactor = 1 + (adjustmentFactor - 1) * handsWeight;

        // Compute the adjusted equity and ensure it remains between 0 and 1.
        double adjustedEquity = baseEquity * weightedAdjustmentFactor;
        adjustedEquity = Math.Max(0, Math.Min(1, adjustedEquity));

        return adjustedEquity;
    }

    /// <summary>
    /// Returns a weight based on the number of hands played.
    /// For 10 or fewer hands, returns 0 (no adjustment).
    /// For hands greater than 10, returns a value quickly converging to 1.
    /// At 100 hands, the weight is nearly 1.
    /// </summary>
    /// <param name="hands">The number of hands played</param>
    /// <returns>A weight between 0 and 1</returns>
    private static double GetHandsWeight(int hands)
    {
        if (hands <= MIN_HANDS_FOR_ADJUSTMENT)
        {
            return 0;
        }
        else
        {
            double weight = 1 - Math.Exp(-HANDS_EXP_GROWTH_RATE * (hands - MIN_HANDS_FOR_ADJUSTMENT));
            return Math.Min(weight, 1);
        }
    }
}
