using System;

namespace Game.Games.TexasHoldem.Model;

[Flags]
public enum PokerDebugFlags
{
    None = 0,
    StateResults = 1,
    MatchResults = 2,
    ActionRecognition = 4,
    All = StateResults | MatchResults | ActionRecognition
}