namespace Game.Common.Model;

[Flags]
public enum PokerDebugFlags
{
    None = 0,
    StateResults = 1,
    MatchResults = 2,
    ActionRecognition = 4,
    PlayerStatistics = 8,
    Ev = 16,
    All = StateResults | MatchResults | ActionRecognition | PlayerStatistics | Ev
}