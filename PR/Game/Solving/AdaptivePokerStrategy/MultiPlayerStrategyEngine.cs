using System.Collections.Generic;
using System.Linq;
using CfrSolver.Model;
using Game.Common.Model;
using Game.Utils;

namespace Game.Solving.AdaptivePokerStrategy;

/// <summary>
/// MultiPlayerStrategyEngine generalizes the heads-up CFR-based decision process to a multi-player context.
/// It combines pairwise strategies (against each opponent) with an aggregated composite strategy,
/// then applies multiway scaling factors to account for increased risk in multi-player pots.
/// 
/// If there is only one opponent, it falls back to using the heads-up engine directly.
/// </summary>
public class MultiPlayerStrategyEngine
{
    // The heads-up decision engine (from previous solutions).
    private readonly PokerDecisionEngine _headsUpEngine;
    // Board generator used by the heads-up engine.
    // Configuration for multi-player scaling.
    private readonly MultiPlayerScalingConfig _scalingConfig;

    /// <summary>
    /// Initializes a new instance of the MultiPlayerStrategyEngine.
    /// </summary>
    /// <param name="headsUpEngine">An instance of the heads-up PokerDecisionEngine.</param>
    /// <param name="scalingConfig">Configuration for multi-player adjustments.</param>
    public MultiPlayerStrategyEngine(PokerDecisionEngine headsUpEngine, MultiPlayerScalingConfig scalingConfig)
    {
        _headsUpEngine = headsUpEngine;
        _scalingConfig = scalingConfig;
    }

    /// <summary>
    /// Computes a probability distribution over legal actions for a multi-player game.
    /// If only one opponent is present, the heads-up engine is used directly.
    /// Otherwise, the method:
    /// 1. Computes a heads-up strategy for each opponent (pairwise strategies).
    /// 2. Aggregates opponent stats into a composite opponent.
    /// 3. Blends the pairwise and composite strategies.
    /// 4. Applies a multiway scaling factor to adjust aggressive actions.
    /// </summary>
    /// <param name="mpState">The multi-player game state.</param>
    /// <returns>A dictionary mapping each legal action to its final probability.</returns>
    public Dictionary<string, float> DecideActionMultiPlayer(MultiPlayerGameState mpState)
    {
        var baseState = mpState.BaseGameState;
        int numOpponents = mpState.OpponentStats.Count;

        if (numOpponents == 1)
        {
            return _headsUpEngine.DecideAction(baseState, mpState.OpponentStats[0]);
        }

        List<Dictionary<string, float>> pairwiseStrategies = [];
        foreach (var oppStats in mpState.OpponentStats)
        {
            var strategy = _headsUpEngine.DecideAction(baseState, oppStats);
            pairwiseStrategies.Add(strategy);
        }

        var possibleActions = pairwiseStrategies[0].Keys.ToList();

        var avgPairwiseStrategy = new Dictionary<string, float>();
        foreach (var action in possibleActions)
        {
            avgPairwiseStrategy[action] = pairwiseStrategies.Average(s => s[action]);
        }

        PlayerStatsRelative compositeStats = ComputeCompositeStats(mpState.OpponentStats);
        var compositeStrategy = _headsUpEngine.DecideAction(baseState, compositeStats);

        Dictionary<string, float> blendedStrategy = new Dictionary<string, float>();
        foreach (var action in possibleActions)
        {
            blendedStrategy[action] =
                _scalingConfig.CompositeWeight * compositeStrategy[action] +
                _scalingConfig.PairwiseWeight * avgPairwiseStrategy[action];
        }

        ApplyAggressiveFactor(numOpponents, blendedStrategy);

        ProbabilityUtils.NormalizeProbabilities(blendedStrategy);

        return blendedStrategy;
    }

    private void ApplyAggressiveFactor(int numOpponents, Dictionary<string, float> blendedStrategy)
    {
        double multiwayAggressiveFactor = 1.0 / (1.0 + (numOpponents - 1) * _scalingConfig.AggressiveScalingCoefficient);

        foreach (var action in blendedStrategy.Keys.ToList())
        {
            if (action.StartsWith(nameof(OpType.Raise)[0]) || action.StartsWith(nameof(OpType.All)[0]))
            {
                blendedStrategy[action] = (float)(blendedStrategy[action] * multiwayAggressiveFactor);
            }
        }
    }

    /// <summary>
    /// Computes composite opponent statistics as a weighted average of individual opponent stats.
    /// We weight by the number of hands played by each opponent.
    /// </summary>
    /// <param name="opponentStatsList">A list of opponent statistics.</param>
    /// <returns>A composite PlayerStatsRelative representing the aggregated opponent.</returns>
    private static PlayerStatsRelative ComputeCompositeStats(List<PlayerStatsRelative> opponentStatsList)
    {
        var totalHands = opponentStatsList.Sum(s => s.Hands);
        var vpip = opponentStatsList.Sum(s => s.VPIP * s.Hands) / totalHands;
        var pfr = opponentStatsList.Sum(s => s.PFR * s.Hands) / totalHands;
        var threeBet = opponentStatsList.Sum(s => s.ThreeBet * s.Hands) / totalHands;
        var foldToThreeBet = opponentStatsList.Sum(s => s.FoldToThreeBet * s.Hands) / totalHands;
        var cBetFlop = opponentStatsList.Sum(s => s.CBetFlop * s.Hands) / totalHands;
        var foldToCBetFlop = opponentStatsList.Sum(s => s.FoldToCBetFlop * s.Hands) / totalHands;
        var wtsd = opponentStatsList.Sum(s => s.WTSD * s.Hands) / totalHands;

        return new PlayerStatsRelative(totalHands, vpip, pfr, threeBet, foldToThreeBet, cBetFlop, foldToCBetFlop, wtsd);
    }
}