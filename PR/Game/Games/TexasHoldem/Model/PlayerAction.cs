namespace Game.Games.TexasHoldem.Model;

public class PlayerAction
{
    public string PlayerName { get; set; }  // E.g. "Hero", "Opp1", "Opp2", ...
    public PokerActionType ActionType { get; set; }
    public decimal Amount { get; set; }     // How many chips they put into the pot (if relevant)
    public PokerPhase Phase { get; set; }   // Which phase of the game this action occurred in

    public override string ToString()
    {
        // E.g. "Opp1 -> Fold (Preflop)" or "Hero -> Raise 120 (Flop)"
        return $"{PlayerName} -> {ActionType}{(Amount > 0 ? $" {Amount}" : "")} ({Phase})";
    }
}