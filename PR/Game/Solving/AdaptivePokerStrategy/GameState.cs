using System.Collections.Generic;
using CfrSolver.Model;
using PT.Poker.Model;

namespace Game.Solving.AdaptivePokerStrategy;

/// <summary>
/// Encapsulates the current game state for decision making.
/// </summary>
public record GameState
{
    public int CurrentPlayer { get; set; }
    public int Round { get; set; }
    public List<string> ActionHistory { get; set; }
    public float Pay { get; set; }
    public Card[] PlayerHoleCards { get; set; }
    public BoardInfo Board { get; set; }
    public Card[] FlopCards { get; set; }
    public Card[] TurnCard { get; set; }
    public Card[] RiverCard { get; set; }
    public decimal PlayerStack { get; set; }
}