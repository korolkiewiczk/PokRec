using System.Collections.Generic;
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
    Dictionary<string, int> NicknameToStack,
    bool IsPlayerDecision);