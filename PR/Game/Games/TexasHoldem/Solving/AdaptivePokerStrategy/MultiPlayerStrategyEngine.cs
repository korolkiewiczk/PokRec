using System.Collections.Generic;
using System.Linq;
using CfrSolver.Interfaces;
using CfrSolver.Model;
using Common;

namespace Game.Games.TexasHoldem.Solving.AdaptivePokerStrategy;

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
    private readonly IBoardGenerator _boardGenerator;
    // Configuration for multi-player scaling.
    private readonly MultiPlayerScalingConfig _scalingConfig;

    /// <summary>
    /// Initializes a new instance of the MultiPlayerStrategyEngine.
    /// </summary>
    /// <param name="headsUpEngine">An instance of the heads-up PokerDecisionEngine.</param>
    /// <param name="boardGenerator">The board generator used to obtain board abstractions.</param>
    /// <param name="scalingConfig">Configuration for multi-player adjustments.</param>
    public MultiPlayerStrategyEngine(PokerDecisionEngine headsUpEngine, IBoardGenerator boardGenerator, MultiPlayerScalingConfig scalingConfig)
    {
        _headsUpEngine = headsUpEngine;
        _boardGenerator = boardGenerator;
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

        // Fallback: If only one opponent exists, use the heads-up engine directly.
        if (numOpponents == 1)
        {
            return _headsUpEngine.DecideAction(baseState, mpState.OpponentStats[0]);
        }

        // 1. For each opponent, get a pairwise heads-up strategy.
        List<Dictionary<string, float>> pairwiseStrategies = new List<Dictionary<string, float>>();
        foreach (var oppStats in mpState.OpponentStats)
        {
            var strategy = _headsUpEngine.DecideAction(baseState, oppStats);
            pairwiseStrategies.Add(strategy);
        }

        // Average the pairwise strategies for each legal action.
        Dictionary<string, float> avgPairwiseStrategy = new Dictionary<string, float>();
        foreach (var action in baseState.PossibleActions)
        {
            avgPairwiseStrategy[action] = pairwiseStrategies.Average(s => s[action]);
        }

        // 2. Compute composite opponent stats by aggregating all opponents.
        PlayerStatsRelative compositeStats = ComputeCompositeStats(mpState.OpponentStats);
        var compositeStrategy = _headsUpEngine.DecideAction(baseState, compositeStats);

        // 3. Blend composite and pairwise strategies using configured weights.
        Dictionary<string, float> blendedStrategy = new Dictionary<string, float>();
        foreach (var action in baseState.PossibleActions)
        {
            blendedStrategy[action] =
                (float)(_scalingConfig.CompositeWeight * compositeStrategy[action] +
                        _scalingConfig.PairwiseWeight * avgPairwiseStrategy[action]);
        }

        // 4. Apply a multiway scaling factor for aggressive actions.
        // As the number of opponents increases, reduce aggressive (raise/all-in) actions.
        double multiwayAggressiveFactor = 1.0 / (1.0 + (numOpponents - 1) * _scalingConfig.AggressiveScalingCoefficient);
        foreach (var action in blendedStrategy.Keys.ToList())
        {
            if (action.StartsWith(nameof(OpType.Raise)[0]) || action.StartsWith(nameof(OpType.All)[0]))
            {
                blendedStrategy[action] = (float)(blendedStrategy[action] * multiwayAggressiveFactor);
            }
        }

        // Normalize the final probability distribution.
        float total = blendedStrategy.Values.Sum();
        if (total > 0)
        {
            foreach (var key in blendedStrategy.Keys.ToList())
            {
                blendedStrategy[key] /= total;
            }
        }

        return blendedStrategy;
    }

    /// <summary>
    /// Computes composite opponent statistics as a weighted average of individual opponent stats.
    /// We weight by the number of hands played by each opponent.
    /// </summary>
    /// <param name="opponentStatsList">A list of opponent statistics.</param>
    /// <returns>A composite PlayerStatsRelative representing the aggregated opponent.</returns>
    private PlayerStatsRelative ComputeCompositeStats(List<PlayerStatsRelative> opponentStatsList)
    {
        double totalHands = opponentStatsList.Sum(s => s.Hands);
        double vpip = opponentStatsList.Sum(s => s.VPIP * s.Hands) / totalHands;
        double pfr = opponentStatsList.Sum(s => s.PFR * s.Hands) / totalHands;
        double threeBet = opponentStatsList.Sum(s => s.ThreeBet * s.Hands) / totalHands;
        double foldToThreeBet = opponentStatsList.Sum(s => s.FoldToThreeBet * s.Hands) / totalHands;
        double cBetFlop = opponentStatsList.Sum(s => s.CBetFlop * s.Hands) / totalHands;
        double foldToCBetFlop = opponentStatsList.Sum(s => s.FoldToCBetFlop * s.Hands) / totalHands;
        double wtsd = opponentStatsList.Sum(s => s.WTSD * s.Hands) / totalHands;

        return new PlayerStatsRelative((int)totalHands, vpip, pfr, threeBet, foldToThreeBet, cBetFlop, foldToCBetFlop, wtsd);
    }
}