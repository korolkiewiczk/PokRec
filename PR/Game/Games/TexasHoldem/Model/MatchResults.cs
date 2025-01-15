using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using PT.Poker.Model;

namespace Game.Games.TexasHoldem.Model;

public record MatchResults(
    List<Card> PlayerCards,
    List<Card> Flop,
    List<Card> Turn,
    List<Card> River,
    Place Position,
    Place Opponent,
    List<string> Nicknames,
    List<decimal?> Stacks,
    bool IsPlayerDecision,
    decimal Pot)
{
    public virtual bool Equals(MatchResults? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return PlayerCards.SequenceEqual(other.PlayerCards) &&
               Flop.SequenceEqual(other.Flop) &&
               Turn.SequenceEqual(other.Turn) &&
               River.SequenceEqual(other.River) &&
               Position.Equals(other.Position) &&
               Opponent.Equals(other.Opponent) &&
               Stacks.SequenceEqual(other.Stacks) &&
               IsPlayerDecision == other.IsPlayerDecision &&
               Pot == other.Pot;
        //nicknames not included because it's not used in comparison
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var card in PlayerCards) hash.Add(card);
        foreach (var card in Flop) hash.Add(card);
        foreach (var card in Turn) hash.Add(card);
        foreach (var card in River) hash.Add(card);
        foreach (var stack in Stacks) hash.Add(stack);
        hash.Add(Position);
        hash.Add(Opponent);
        hash.Add(IsPlayerDecision);
        hash.Add(Pot);
        return hash.ToHashCode();
    }
}