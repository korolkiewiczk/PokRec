using System;
using System.Collections.Generic;
using System.Linq;
using CfrSolver.Interfaces;
using CfrSolver.Model;
using CfrSolver.Utils;
using Game.Common.Model;
using Game.Utils;
using PlayerAction = Game.Common.Model.PlayerAction;

namespace Game.Solving.AdaptivePokerStrategy;

/// <summary>
/// PokerDecisionEngine blends strategies from multiple GTO databases and adjusts them based on opponent tendencies.
/// The engine uses an interpolated lookup to obtain strategies weighted by a penalty,
/// and then applies smooth, linear adjustments using opponent statistics (configured via OpponentAdjustmentConfig).
/// </summary>
public class PokerDecisionEngine
{
    private readonly IBoardGenerator _boardGenerator;
    private readonly Node[] _gtoRootNodes;
    private readonly OpponentAdjustmentConfig _opponentAdjustmentConfig;
    private readonly List<int> _possibleRaiseValues;
    private readonly int _monteCarloIterations;

    /// <summary>
    /// Initializes a new instance of the PokerDecisionEngine.
    /// </summary>
    /// <param name="boardGenerator">Provides board abstraction generation.</param>
    /// <param name="gtoRootNodes">An array of GTO root nodes representing different GTO databases.</param>
    /// <param name="opponentAdjustmentConfig">Configuration for opponent adjustment superparameters.</param>
    /// <param name="possibleRaiseValues"></param>
    /// <param name="monteCarloIterations"></param>
    public PokerDecisionEngine(IBoardGenerator boardGenerator, Node[] gtoRootNodes,
        OpponentAdjustmentConfig opponentAdjustmentConfig, List<int> possibleRaiseValues, 
        int monteCarloIterations)
    {
        _boardGenerator = boardGenerator;
        _gtoRootNodes = gtoRootNodes;
        _opponentAdjustmentConfig = opponentAdjustmentConfig;
        _possibleRaiseValues = possibleRaiseValues;
        _monteCarloIterations = monteCarloIterations;
    }

    /// <summary>
    /// Computes a probability distribution over legal actions by combining strategies
    /// from multiple GTO databases (weighted by inverse penalty) and adjusting based on opponent stats.
    /// </summary>
    /// <param name="gameState">The current game state.</param>
    /// <param name="opponentStats">Opponent statistics for adaptation.</param>
    /// <returns>A dictionary mapping each legal action to its computed probability.</returns>
    public Dictionary<string, float> DecideAction(GameState gameState, PlayerStatsRelative opponentStats = null)
    {
        // Step 1: Obtain (or generate) the board abstraction.
        var board = gameState.BoardInfo ?? _boardGenerator.GenerateBoardAbstraction(gameState.PlayerCards);

        int handAbstraction = board.Hand;
        string[] actionsArray = gameState.ActionHistory.ToArray();

        // Step 2: Interpolate strategies from multiple GTO databases.
        // Each strategy is weighted by 1/penalty.
        Dictionary<string, float> combinedStrategy = new Dictionary<string, float>();
        float totalWeight = 0.0f;

        foreach (var rootNode in _gtoRootNodes)
        {
            var (node, penalty) = rootNode.GetInterpolatedNodeForActionSequence(actionsArray);
            if (penalty >= NodeExtensions.MaxPenalty) continue;
            float weight = 1.0f / penalty;
            Dictionary<string, float> baseStrategy = node.GetAverageStrategyWithActions(handAbstraction);

            foreach (var kvp in baseStrategy)
            {
                combinedStrategy.TryAdd(kvp.Key, 0);
                combinedStrategy[kvp.Key] += kvp.Value * weight;
            }

            totalWeight += weight;
        }

        Dictionary<string, float> actionProbabilities;
        if (totalWeight > 0)
        {
            ProbabilityUtils.NormalizeProbabilities(combinedStrategy, totalWeight);
            actionProbabilities = combinedStrategy;
        }
        else
        {
            actionProbabilities = GetEquityBasedStrategy(gameState, opponentStats, _possibleRaiseValues, _monteCarloIterations);
        }

        // Step 3: Adjust the strategy based on opponent statistics.
        if (opponentStats != null)
        {
            actionProbabilities = AdjustStrategyForOpponent(actionProbabilities, opponentStats);
        }

        // Step 4: Normalize the final distribution.
        ProbabilityUtils.NormalizeProbabilities(actionProbabilities);

        return actionProbabilities;
    }

    /// <summary>
    /// Computes action probabilities based on equity calculation when a full GTO strategy is not available.
    /// Candidate actions always include: Fold ("F"), Call ("C"), Raise ("R{x}") for each value in possibleRaiseValues, and All–In ("A{amount}").
    /// </summary>
    /// <param name="gameState">The current state of the game.</param>
    /// <param name="opponentStats">The opponent's relative statistics.</param>
    /// <param name="possibleRaiseValues">
    /// The list of possible raise amounts. These values are typically computed based on the current pot (gameState.Pay).
    /// </param>
    /// <param name="monteCarloIterations"></param>
    /// <returns>A dictionary mapping each candidate action label to its probability weight.</returns>
    private static Dictionary<string, float> GetEquityBasedStrategy(
        GameState gameState,
        PlayerStatsRelative opponentStats,
        List<int> possibleRaiseValues, int monteCarloIterations)
    {
        // Convert the historical action strings into PlayerAction objects.
        var baseActions = ExtractPlayerActions(gameState);

        // Prepare the opponent configuration.
        var place = new Place();
        place.Add(0); // Add one opponent position.

        // Compute the Monte Carlo simulation result based on the current board and player's hole cards.
        var (monteCarloResult, _) = PokerHelper.SolvePlayerLayout(
            gameState.PlayerCards,
            place,
            gameState.FlopCards,
            gameState.TurnCard,
            gameState.RiverCard,
            [opponentStats], monteCarloIterations);

        // Build the candidate actions list as tuples: (ActionType, Amount, Label)
        var candidateActionsAndLabels = new List<(PokerActionType ActionType, decimal Amount, string Label)>
        {
            // Fold candidate
            (PokerActionType.Fold, 0, "F"),
            // Call candidate
            (PokerActionType.Call, 0, "C")
        };

        // Raise candidates: one candidate per amount in possibleRaiseValues.
        if (possibleRaiseValues != null)
        {
            foreach (var raise in possibleRaiseValues)
            {
                candidateActionsAndLabels.Add((PokerActionType.Raise, raise, $"R{raise}"));
            }
        }

        // All–In candidate: using the player's entire stack.
        candidateActionsAndLabels.Add((PokerActionType.AllIn, gameState.PlayerStack, $"A{gameState.PlayerStack}"));

        // Evaluate each candidate action by appending it to the base action history.
        var evBasedProbabilities = new Dictionary<string, float>();
        float totalProbabilityWeight = 0.0f;

        foreach (var candidate in candidateActionsAndLabels)
        {
            // Create a new action history that includes the candidate action.
            var candidateActionHistory = new List<PlayerAction>(baseActions)
            {
                new(1, candidate.ActionType, candidate.Amount, PokerPhase.Preflop)
            };

            // Calculate the EV for the candidate action.
            var evResult = EvCalculator.CalculateEv(
                candidateActionHistory,
                monteCarloResult.Value,
                gameState.Pot
            );

            // Map the EV (specifically netEv) to a probability weight via an exponential function.
            float probabilityWeight = (float)Math.Exp((double)evResult.NetEv);
            evBasedProbabilities[candidate.Label] = probabilityWeight;
            totalProbabilityWeight += probabilityWeight;
        }

        // Normalize the probabilities.
        if (totalProbabilityWeight > 0)
        {
            ProbabilityUtils.NormalizeProbabilities(evBasedProbabilities, totalProbabilityWeight);
        }
        else
        {
            // If all EVs are negative, fall back to an equal distribution.
            int totalActions = evBasedProbabilities.Count;
            foreach (var key in evBasedProbabilities.Keys.ToList())
            {
                evBasedProbabilities[key] = 1.0f / totalActions;
            }
        }

        return evBasedProbabilities;
    }

    private static List<PlayerAction> ExtractPlayerActions(GameState gameState)
    {
        var baseActions = new List<PlayerAction>();
        foreach (var actionStr in gameState.ActionHistory)
        {
            if (string.IsNullOrEmpty(actionStr))
                continue;

            // The first character represents the action type.
            char actionChar = actionStr[0];

            // Parse any trailing characters as the amount.
            decimal amount = 0;
            if (actionStr.Length > 1)
            {
                var amountPart = actionStr[1..];
                if (!decimal.TryParse(amountPart, out amount))
                {
                    amount = 0;
                }
            }

            PokerActionType actionType = actionChar switch
            {
                'F' => PokerActionType.Fold,
                'C' => PokerActionType.Call,
                'R' => PokerActionType.Raise,
                'A' => PokerActionType.AllIn,
                _   => PokerActionType.Check
            };

            baseActions.Add(new PlayerAction(1, actionType, amount, PokerPhase.Preflop));
        }

        return baseActions;
    }

    /// <summary>
    /// Adjusts the base strategy using smooth linear interpolation based on all opponent statistics.
    /// Different multipliers are applied to aggressive actions ("R"/"A"), calling ("C"), and folding ("F").
    /// </summary>
    private Dictionary<string, float> AdjustStrategyForOpponent(Dictionary<string, float> strategy,
        PlayerStatsRelative opponentStats)
    {
        // Helper: Linear interpolation.
        double Lerp(double a, double b, double t) => a + (b - a) * Math.Clamp(t, 0.0, 1.0);

        // --- Aggressive actions (Raise/All-in: "R"/"A") ---
        // Use FoldToThreeBet, WTSD, and ThreeBet.
        double foldTo3BetFactor = Lerp(1.0, 1.2, opponentStats.FoldToThreeBet / 100.0) *
                                  _opponentAdjustmentConfig.FoldToThreeBetWeight;
        double wtsdFactor = opponentStats.WTSD >= 50.0
            ? Lerp(1.0, 0.8, (opponentStats.WTSD - 50.0) / 10.0) * _opponentAdjustmentConfig.WTSDWeight
            : Lerp(1.0, 1.1, (50.0 - opponentStats.WTSD) / 10.0) * _opponentAdjustmentConfig.WTSDWeight;
        double threeBetFactor = Lerp(1.0, 0.95, opponentStats.ThreeBet / 100.0) *
                                _opponentAdjustmentConfig.ThreeBetWeight;
        double aggressiveAdjustment = foldTo3BetFactor * wtsdFactor * threeBetFactor;

        // --- Calling actions ("C") ---
        // Use VPIP and CBetFlop.
        double vpipFactor = Lerp(1.0, 0.9, opponentStats.VPIP / 100.0) * _opponentAdjustmentConfig.VPIPWeight;
        double cbetFlopFactor = Lerp(1.0, 1.05, opponentStats.CBetFlop / 100.0) *
                                _opponentAdjustmentConfig.CBetFlopWeight;
        double callAdjustment = vpipFactor * cbetFlopFactor;

        // --- Folding actions ("F") ---
        // Use PFR and FoldToCBetFlop.
        double pfrFactor = Lerp(1.0, 1.05, opponentStats.PFR / 100.0) * _opponentAdjustmentConfig.PFRWeight;
        double foldToCBetFlopFactor = Lerp(1.0, 1.1, opponentStats.FoldToCBetFlop / 100.0) *
                                      _opponentAdjustmentConfig.FoldToCBetFlopWeight;
        double foldAdjustment = pfrFactor * foldToCBetFlopFactor;

        // Create a new dictionary to hold adjusted probabilities.
        var adjustedStrategy = new Dictionary<string, float>(strategy);

        foreach (var action in adjustedStrategy.Keys.ToList())
        {
            // Adjust based on action type.
            if (action.StartsWith(nameof(OpType.Raise)[0]) || action.StartsWith(nameof(OpType.All)[0]))
            {
                adjustedStrategy[action] = (float) (adjustedStrategy[action] * aggressiveAdjustment);
            }
            else if (action.StartsWith(nameof(OpType.Call)[0]))
            {
                adjustedStrategy[action] = (float) (adjustedStrategy[action] * callAdjustment);
            }
            else if (action.StartsWith(nameof(OpType.Fold)[0]))
            {
                adjustedStrategy[action] = (float) (adjustedStrategy[action] * foldAdjustment);
            }
            // Other action types remain unmodified.
        }

        return adjustedStrategy;
    }
}