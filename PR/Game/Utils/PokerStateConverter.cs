using System.Collections.Generic;
using System.Linq;
using CfrSolver.Model;
using Game.Common.Model;
using Game.Solving.AdaptivePokerStrategy;
using PT.Poker.Model;

namespace Game.Utils
{
    /// <summary>
    /// Helper class for converting PokerResults to GameState
    /// </summary>
    public static class PokerStateConverter
    {
        /// <summary>
        /// Converts PokerResults into GameState format
        /// </summary>
        /// <param name="pokerResults">The poker results to convert</param>
        /// <param name="gameActions">The list of player actions in the game</param>
        /// <returns>A new GameState instance containing the converted data</returns>
        public static GameState ConvertToGameState(PokerResults pokerResults, List<Common.Model.PlayerAction> gameActions, 
            StartingBets startingBets)
        {
            var gameState = new GameState
            {
                CurrentPlayer = (int)pokerResults.PokerPosition,
                Round = (int)pokerResults.Phase,
                ActionHistory = gameActions.Select(a => a.ActionType.ToString()).ToList(),
                Pay = (float)pokerResults.EvResult.Pot,
                PlayerHoleCards = pokerResults.MatchResults.PlayerCards.ToArray(),
                Board = new BoardInfo
                {
                    Hand = pokerResults.BestLayout != null ? (int)pokerResults.BestLayout : 0
                },
                FlopCards = pokerResults.MatchResults.Flop.ToArray(),
                TurnCard = pokerResults.MatchResults.Turn.ToArray(),
                RiverCard = pokerResults.MatchResults.River.ToArray()
            };

            // Initialize player stack from match results
            if (pokerResults.MatchResults.Stacks.Count > 0)
            {
                gameState.PlayerStack = pokerResults.MatchResults.Stacks[0] / startingBets.SmallBlind ?? 0;
            }

            return gameState;
        }
    }
} 