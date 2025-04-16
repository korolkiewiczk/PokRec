using System.Collections.Generic;
using CfrSolver.Model;
using PT.Poker.Model;

namespace Game.Solving.AdaptivePokerStrategy;

/// <summary>
/// Encapsulates the current game state for decision making.
/// </summary>
public record GameState
{
    public List<string> ActionHistory { get; init; }
    public decimal Pot { get; init; }
    public Card[] PlayerCards { get; init; }
    public Card[] FlopCards { get; init; } = [];
    public Card[] TurnCard { get; init; } = [];
    public Card[] RiverCard { get; init; } = [];
    public BoardInfo BoardInfo { get; init; }
    public decimal PlayerStack { get; init; }
}