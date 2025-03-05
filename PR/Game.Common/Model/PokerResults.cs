using System.Collections.Immutable;
using PT.Algorithm.Model;
using PT.Poker.Model;

namespace Game.Common.Model;

public record PokerResults(
    ReconResults ReconResults,
    MatchResults MatchResults,
    MonteCarloResult? MonteCarloResult,
    PokerLayouts? BestLayout,
    PokerPosition? PokerPosition,
    IImmutableList<bool> OpponentsInGame,
    PokerPhase Phase,
    bool IsCorrectPot,
    EvResult EvResult);