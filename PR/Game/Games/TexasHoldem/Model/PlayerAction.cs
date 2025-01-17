namespace Game.Games.TexasHoldem.Model;

public record PlayerAction(int PlayerIndex, PokerActionType ActionType, decimal Amount, PokerPhase Phase)
{
    public override string ToString()
    {
        // E.g. "Opp1 -> Fold (Preflop)" or "Hero -> Raise 120 (Flop)"
        return $"Player{PlayerIndex} -> {ActionType}{(Amount > 0 ? $" {Amount}" : "")} ({Phase})";
    }
}