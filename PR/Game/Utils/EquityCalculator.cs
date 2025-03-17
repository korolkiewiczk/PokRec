using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Common.Model;
using PT.Algorithm;
using PT.Algorithm.Model;
using PT.Poker.Model;

namespace Game.Utils
{
    public static class EquityCalculator
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType!);
            
        private static Dictionary<string, double> _equityCache;

        /// <summary>
        /// Precomputes hand equities for common board states in the background
        /// </summary>
        /// <param name="iterations">Number of iterations per hand</param>
        public static void PrecomputeHandEquitiesInBackground(int iterations)
        {
            Task.Run(() =>
            {
                Log.Info($"Starting background precomputation of hand equities...");

                GetOrGenerateHandEquities(iterations);
                
                Log.Info($"Finished precomputing hand equities for preflop.");
            });
        }

        /// <summary>
        /// Performs a two-stage equity calculation:
        /// 1. First gets or calculates basic equity for all possible hands (using cache if available)
        /// 2. Then uses those equities as weights for a more accurate simulation
        /// </summary>
        /// <param name="randomSetDefinition">The parameters for the simulation</param>
        /// <param name="weightedIterations">Number of iterations for the weighted equity calculation</param>
        /// <param name="stats">List of players stats</param>
        /// <param name="basicIterations">Number of iterations for the basic equity calculation (if not cached)</param>
        /// <returns>The Monte Carlo result from the weighted simulation</returns>
        public static MonteCarloResult CalculateTwoStageEquityWithStats(
            RandomSetDefinition randomSetDefinition, int weightedIterations, List<PlayerStatsRelative> stats,
            int basicIterations = 1000)
        {
            var handEquities = GetOrGenerateHandEquities(basicIterations);
            var avgStats = PlayerHandEquityAdjuster.ComputeWeightedAverageStats(stats);
            var adjHandEquities = PlayerHandEquityAdjuster.AdjustHandEquities(handEquities, avgStats);

            // Convert to simulation parameters with the calculated equities
            var parameters = new SimulationParameters(randomSetDefinition, adjHandEquities);

            // Run the weighted simulation
            return CalculateWeightedEquity(parameters, weightedIterations);
        }

        /// <summary>
        /// Calculates the equity for a given hand using the weighted Monte Carlo simulation
        /// </summary>
        /// <param name="simulationParameters">The parameters for the simulation, including hand equities</param>
        /// <param name="iterations">Number of Monte Carlo iterations</param>
        /// <returns>The Monte Carlo result containing equity values</returns>
        private static MonteCarloResult CalculateWeightedEquity(SimulationParameters simulationParameters, int iterations = 10000)
        {
            var monteCarlo = new WeightedMonteCarlo<WeightedCardSet, SimulationParameters>(iterations, simulationParameters);
            return monteCarlo.Solve();
        }

        /// <summary>
        /// Gets hand equities from cache or generates them if not cached
        /// </summary>
        /// <param name="iterations">Number of iterations per hand</param>
        /// <returns>A dictionary mapping hand strings to their equity values</returns>
        private static Dictionary<string, double> GetOrGenerateHandEquities(int iterations)
        {
            if (_equityCache == null)
            {
                var equities = GenerateHandEquities(iterations);
                _equityCache = equities;
            }

            return _equityCache;
        }

        /// <summary>
        /// Generates equity values for all possible starting hands in Texas Hold'em
        /// </summary>
        /// <param name="iterations">Number of iterations per hand</param>
        /// <returns>A dictionary mapping hand strings to their equity values</returns>
        private static Dictionary<string, double> GenerateHandEquities(int iterations)
        {
            var result = new Dictionary<string, double>();
            
            // Generate all possible 2-card combinations
            var allPossibleHands = GenerateAllPossibleStartingHands();
            
            Log.Info($"Generating equities for {allPossibleHands.Count} possible hands...");
            
            // For each hand, run a Monte Carlo simulation to determine its equity
            foreach (var hand in allPossibleHands)
            {
                // Create a new RandomSetDefinition with this hand
                var handDefinition = new RandomSetDefinition
                {
                    MyLayout = hand.layout,
                    NumOfPlayers = 2,
                    Board = []
                };
                
                // Run the standard Monte Carlo simulation
                var monteCarlo = new MonteCarlo<CardSet, RandomSetDefinition>(iterations, handDefinition);
                var equity = monteCarlo.Solve();
                
                // Store the equity (win probability) for this hand
                // We use Better + Exact/2 as the equity value (probability of winning or tying)
                var equityValue = equity.Better + equity.Exact / 2;
                result[hand.key] = equityValue;
                
                // Log progress for long-running operations
                Log.Debug($"Calculated equity for hand {hand.key}: {equityValue:F4}");
            }
                        
            Log.Info($"Finished generating equities for {result.Count} hands.");
            return result;
        }

        /// <summary>
        /// Generates all possible 2-card starting hands that don't conflict with the board
        /// </summary>
        /// <returns>A list of all possible starting hands with their string representation</returns>
        private static List<(string key, CardLayout layout)> GenerateAllPossibleStartingHands()
        {
            var result = new List<(string key, CardLayout layout)>();
            
            // Generate all possible 2-card combinations
            for (var firstCardType = 0; firstCardType < 13; firstCardType++)
            {
                for (var firstCardColor = 0; firstCardColor < 4; firstCardColor++)
                {
                    var firstCard = new Card((CardColor)firstCardColor, (CardType)firstCardType);
                    
                    // Start the second card iteration from a position that ensures no duplicates
                    // We'll use a lexicographical ordering where:
                    // 1. First compare by card type (2,3,4,...,A)
                    // 2. If types are equal, compare by color (clubs, diamonds, hearts, spades)
                    for (var secondCardType = firstCardType; secondCardType < 13; secondCardType++)
                    {
                        // If the card types are the same, start from the next color
                        // Otherwise start from the first color
                        var startColor = (secondCardType == firstCardType) ? firstCardColor + 1 : 0;
                        
                        for (var secondCardColor = startColor; secondCardColor < 4; secondCardColor++)
                        {
                            var secondCard = new Card((CardColor)secondCardColor, (CardType)secondCardType);
                            
                            // Create the hand
                            var cards = new[] { firstCard, secondCard };
                            var layout = new CardLayout(cards);
                            
                            // Generate a key for this hand using the Card.ToEString() method
                            var key = $"{firstCard.ToEString()},{secondCard.ToEString()}";
                            
                            result.Add((key, layout));
                        }
                    }
                }
            }

            return result;
        }
    }
} 