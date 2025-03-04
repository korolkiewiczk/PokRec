using System.Collections.Generic;
using Common;

namespace Game.Games.TexasHoldem.Solving.AdaptivePokerStrategy;

/// <summary>
/// Game state for a multi-player situation.
/// Contains the base game state used for the heads-up engine (our hole cards, board, actions, etc.)
/// and a list of opponent statistics.
/// </summary>
public class MultiPlayerGameState
{
    /// <summary>
    /// The base game state for our decision engine.
    /// This should be the same type as used by the heads-up PokerDecisionEngine.
    /// </summary>
    public PokerDecisionEngine.GameState BaseGameState { get; set; }

    /// <summary>
    /// A list of statistics for each opponent currently in the hand.
    /// </summary>
    public List<PlayerStatsRelative> OpponentStats { get; set; } = new List<PlayerStatsRelative>();
}