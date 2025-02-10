namespace Game.Games.TexasHoldem.Model;

    public record PlayerAction(int PlayerIndex, PokerActionType ActionType, decimal Amount, PokerPhase Phase, string Id="")
{
    public override string ToString()
    {
        return $"Player{PlayerIndex} -> {ActionType}{(Amount > 0 ? $" {Amount}" : "")} ({Phase}) {Id}";
    }
}