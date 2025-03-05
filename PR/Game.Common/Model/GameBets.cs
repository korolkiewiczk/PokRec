using System.Collections.Immutable;

namespace Game.Common.Model;

public record StartingBets(decimal Ante, decimal SmallBlind, decimal BigBlind);
public record GameBets(IImmutableList<PlayerAction> Actions, StartingBets StartingBets);