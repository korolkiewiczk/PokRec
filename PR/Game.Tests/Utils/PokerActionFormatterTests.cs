using Game.Common.Model;
using Game.Utils;

namespace Game.Tests.Utils;

public class PokerActionFormatterTests
{
    [Fact]
    public void FormatActions_ShouldReturnCorrectSequence_ForBasicActions()
    {
        var actions = new List<PlayerAction>
        {
            new(0, PokerActionType.Put, 1000, PokerPhase.Preflop), // Small blind
            new(1, PokerActionType.Put, 2000, PokerPhase.Preflop), // Big blind
            new(0, PokerActionType.Raise, 4000, PokerPhase.Preflop),
            new(1, PokerActionType.Call, 4000, PokerPhase.Preflop),
            new(0, PokerActionType.Check, 0, PokerPhase.Flop),
            new(1, PokerActionType.Check, 0, PokerPhase.Flop)
        };

        string result = PokerActionFormatter.FormatActions(actions, 1000);
        Assert.Equal("R1,R1,R2,C,C,C", result);
    }

    [Fact]
    public void FormatActions_ShouldHandleAllInCorrectly()
    {
        var actions = new List<PlayerAction>
        {
            new(0, PokerActionType.Put, 1000, PokerPhase.Preflop),
            new(1, PokerActionType.Put, 2000, PokerPhase.Preflop),
            new(0, PokerActionType.Raise, 4000, PokerPhase.Preflop),
            new(1, PokerActionType.AllIn, 20000, PokerPhase.Preflop)
        };

        string result = PokerActionFormatter.FormatActions(actions, 1000);
        Assert.Equal("R1,R1,R2,A16", result);
    }

    [Fact]
    public void FormatActions_ShouldHandleFold()
    {
        var actions = new List<PlayerAction>
        {
            new(0, PokerActionType.Put, 1000, PokerPhase.Preflop),
            new(1, PokerActionType.Put, 2000, PokerPhase.Preflop),
            new(0, PokerActionType.Fold, 0, PokerPhase.Preflop)
        };

        string result = PokerActionFormatter.FormatActions(actions, 1000);
        Assert.Equal("R1,R1,F", result);
    }

    [Fact]
    public void FormatActions_ShouldHandleMultipleRaises()
    {
        var actions = new List<PlayerAction>
        {
            new(0, PokerActionType.Put, 1000, PokerPhase.Preflop),
            new(1, PokerActionType.Put, 2000, PokerPhase.Preflop),
            new(0, PokerActionType.Raise, 5900, PokerPhase.Preflop),
            new(1, PokerActionType.Raise, 12000, PokerPhase.Preflop),
            new(0, PokerActionType.Call, 10000, PokerPhase.Preflop),
            new(1, PokerActionType.Bet, 10000, PokerPhase.Preflop)
        };

        string result = PokerActionFormatter.FormatActions(actions, 1000);
        Assert.Equal("R1,R1,R4,R6,C,R10", result);
    }
    
    [Fact]
    public void FormatActions_ShouldHandleMultipleRaisesSmallAmount()
    {
        var actions = new List<PlayerAction>
        {
            new(0, PokerActionType.Put, 1000, PokerPhase.Preflop),
            new(1, PokerActionType.Put, 2000, PokerPhase.Preflop),
            new(0, PokerActionType.Bet, 2001, PokerPhase.Preflop),
            new(1, PokerActionType.Raise, 2002, PokerPhase.Preflop),
            new(0, PokerActionType.Call, 1, PokerPhase.Preflop)
        };

        string result = PokerActionFormatter.FormatActions(actions, 1000);
        Assert.Equal("R1,R1,R0,R0,C", result);
    }
    
    [Fact]
    public void FormatActions_ShouldHandleMultipleRaisesWithAnte()
    {
        var actions = new List<PlayerAction>
        {
            new(0, PokerActionType.Put, 1000, PokerPhase.Preflop),
            new(1, PokerActionType.Put, 100, PokerPhase.Preflop),
            new(0, PokerActionType.Raise, 4000, PokerPhase.Preflop),
            new(1, PokerActionType.Call, 0, PokerPhase.Preflop)
        };

        string result = PokerActionFormatter.FormatActions(actions, 1000);
        Assert.Equal("R1,R0,R3,C", result);
    }
}