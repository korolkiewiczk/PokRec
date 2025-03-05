using System.Collections.Generic;
using System.Linq;
using CfrSolver.Interfaces;
using CfrSolver.Model;
using CfrSolver.Utils;
using Game.Common.Model;
using PT.Poker.Model;

namespace Game.Solving.AdaptivePokerStrategy
{
    /// <summary>
    /// PokerDecisionEngine blends strategies from multiple GTO databases and adjusts them based on opponent tendencies.
    /// The engine uses an interpolated lookup to obtain strategies weighted by a penalty,
    /// and then applies smooth, linear adjustments using opponent statistics (configured via OpponentAdjustmentConfig).
    /// </summary>
    public class PokerDecisionEngine
    {
        private readonly IBoardGenerator _boardGenerator;
        private readonly Node[] _gtoRootNodes; // Multiple GTO databases
        private readonly OpponentAdjustmentConfig _opponentAdjustmentConfig;

        /// <summary>
        /// Initializes a new instance of the PokerDecisionEngine.
        /// </summary>
        /// <param name="boardGenerator">Provides board abstraction generation.</param>
        /// <param name="gtoRootNodes">An array of GTO root nodes representing different GTO databases.</param>
        /// <param name="opponentAdjustmentConfig">Configuration for opponent adjustment superparameters.</param>
        public PokerDecisionEngine(IBoardGenerator boardGenerator, Node[] gtoRootNodes,
            OpponentAdjustmentConfig opponentAdjustmentConfig)
        {
            _boardGenerator = boardGenerator;
            _gtoRootNodes = gtoRootNodes;
            _opponentAdjustmentConfig = opponentAdjustmentConfig;
        }

        /// <summary>
        /// Encapsulates the current game state for decision making.
        /// </summary>
        public class GameState
        {
            public int CurrentPlayer { get; set; }
            public int Round { get; set; }
            public List<string> ActionHistory { get; set; } = [];
            public float Pay { get; set; }
            public List<string> PossibleActions { get; set; } = [];
            public Card[] PlayerHoleCards { get; set; }
            public BoardInfo Board { get; set; }
        }

        /// <summary>
        /// Computes a probability distribution over legal actions by combining strategies
        /// from multiple GTO databases (weighted by inverse penalty) and adjusting based on opponent stats.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="opponentStats">Opponent statistics for adaptation.</param>
        /// <returns>A dictionary mapping each legal action to its computed probability.</returns>
        public Dictionary<string, float> DecideAction(GameState gameState, PlayerStatsRelative opponentStats)
        {
            // Step 1: Obtain (or generate) the board abstraction.
            var board = gameState.Board ?? _boardGenerator.GenerateBoardAbstraction(gameState.PlayerHoleCards);

            int handAbstraction = board.Hand;
            string[] actionsArray = gameState.ActionHistory.ToArray();
            int numActions = gameState.PossibleActions.Count;

            // Step 2: Interpolate strategies from multiple GTO databases.
            // Each strategy is weighted by 1/penalty.
            float[] combinedStrategy = new float[numActions];
            float totalWeight = 0.0f;

            foreach (var rootNode in _gtoRootNodes)
            {
                var (node, penalty) = rootNode.GetInterpolatedNodeForActionSequence(actionsArray);
                float weight = 1.0f / penalty;
                float[] baseStrategy =
                    node.GetAverageStrategy(handAbstraction);

                // Ensure strategy length matches the legal actions.
                if (baseStrategy.Length != numActions || baseStrategy.Length == 0)
                {
                    baseStrategy = new float[numActions];
                    for (int i = 0; i < numActions; i++)
                        baseStrategy[i] = 1.0f / numActions;
                }

                for (int i = 0; i < numActions; i++)
                {
                    combinedStrategy[i] += baseStrategy[i] * weight;
                }

                totalWeight += weight;
            }

            if (totalWeight > 0)
            {
                for (int i = 0; i < combinedStrategy.Length; i++)
                {
                    combinedStrategy[i] /= totalWeight;
                }
            }

            // Map combined strategy to the current legal actions.
            Dictionary<string, float> actionProbabilities = new Dictionary<string, float>();
            for (int i = 0; i < numActions; i++)
            {
                actionProbabilities[gameState.PossibleActions[i]] = combinedStrategy[i];
            }

            // Step 3: Adjust the strategy based on opponent statistics.
            actionProbabilities = AdjustStrategyForOpponent(actionProbabilities, opponentStats);

            // Step 4: Normalize the final distribution.
            float totalProbability = actionProbabilities.Values.Sum();
            if (totalProbability > 0)
            {
                foreach (var key in actionProbabilities.Keys.ToList())
                {
                    actionProbabilities[key] /= totalProbability;
                }
            }

            return actionProbabilities;
        }

        /// <summary>
        /// Adjusts the base strategy using smooth linear interpolation based on all opponent statistics.
        /// Different multipliers are applied to aggressive actions ("R"/"A"), calling ("C"), and folding ("F").
        /// </summary>
        private Dictionary<string, float> AdjustStrategyForOpponent(Dictionary<string, float> strategy,
            PlayerStatsRelative opponentStats)
        {
            // Helper: Linear interpolation.
            double Lerp(double a, double b, double t) => a + (b - a) * t;

            // --- Aggressive actions (Raise/All-in: "R"/"A") ---
            // Use FoldToThreeBet, WTSD, and ThreeBet.
            double foldTo3BetFactor = Lerp(1.0, 1.2, opponentStats.FoldToThreeBet / 100.0) *
                                      _opponentAdjustmentConfig.FoldToThreeBetWeight;
            double wtsdFactor = opponentStats.WTSD >= 50.0
                ? Lerp(1.0, 0.8, (opponentStats.WTSD - 50.0) / 10.0) * _opponentAdjustmentConfig.WTSDWeight
                : Lerp(1.0, 1.1, (50.0 - opponentStats.WTSD) / 10.0) * _opponentAdjustmentConfig.WTSDWeight;
            double threeBetFactor = Lerp(1.0, 0.95, opponentStats.ThreeBet / 100.0) * _opponentAdjustmentConfig.ThreeBetWeight;
            double aggressiveAdjustment = foldTo3BetFactor * wtsdFactor * threeBetFactor;

            // --- Calling actions ("C") ---
            // Use VPIP and CBetFlop.
            double vpipFactor = Lerp(1.0, 0.9, opponentStats.VPIP / 100.0) * _opponentAdjustmentConfig.VPIPWeight;
            double cbetFlopFactor = Lerp(1.0, 1.05, opponentStats.CBetFlop / 100.0) * _opponentAdjustmentConfig.CBetFlopWeight;
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
}