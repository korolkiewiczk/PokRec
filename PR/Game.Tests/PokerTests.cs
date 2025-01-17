using System.Drawing;
using System.Reflection;
using Common;
using Game.Games.TexasHoldem.Model;
using Game.Games.TexasHoldem.Solving;
using Game.Games.TexasHoldem.Utils;
using Game.MultiRegionMatchers;
using Game.RegionMatchers;
using Xunit.Abstractions;
using Game.Tests.Helpers;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;

// for Rectangle

// Adjust namespaces to your actual project

namespace Game.Tests;

public class PokerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PokerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        // Configure log4net to use our custom appender
        var hierarchy = (Hierarchy) LogManager.GetRepository();
        hierarchy.Root.RemoveAllAppenders(); // Clear existing appenders

        var appender = new TestOutputHelperAppender(testOutputHelper);
        appender.ActivateOptions();

        hierarchy.Root.AddAppender(appender);
        hierarchy.Root.Level = Level.All;
        hierarchy.Configured = true;
    }

    [Fact]
    public void Solve_ShouldInferActionsProperly_WhenSteppingThroughPhases()
    {
        // Arrange
        var poker = new Poker(new Board
        {
            Settings =
            [
                new KeyValuePair<string, string>(nameof(PokerBoardSettingsParser.Players), "6")
            ]
        });

        // 0) pre-Preflop (before poker next hand) 
        var prepreflopState = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I()},
            {nameof(Flop), I("C6c")},
            {nameof(Turn), I("C8c")},
            {nameof(River), I("C3c")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I("btn")},
            {"Position6", I()},
            {"Stack1", I("5538")},
            {"Stack2", I("9540")},
            {"Stack3", I("9182")},
            {"Stack4", I("14181")},
            {"Stack5", I("7422")},
            {"Stack6", I("5827")},
            {nameof(Pot), I("460")}
        };

        // 1a) Preflop a
        var preflopState1 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5478")},
            {"Stack2", I("9430")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("210")},
            {"Opponent1", I("cards")},
            {"Opponent2", I("cards")},
            {"Opponent3", I("cards")},
            {"Opponent4", I("cards")},
            {"Opponent5", I("cards")}
        };

        // 1b) Preflop b
        var preflopState2 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5478")},
            {"Stack2", I("9430")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("210")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
            {nameof(Decision), I("bar")}
        };

        // 1c) Preflop c
        var preflopState3 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5428")},
            {"Stack2", I("9430")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("260")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
        };


        // 2a) Flop a
        var flopState1 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {nameof(Flop), I("Jc", "C5h", "Ks")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5428")},
            {"Stack2", I("9430")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("260")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
            {nameof(Decision), I("bar")}
        };

        // 2b) Flop b
        var flopState2 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {nameof(Flop), I("Jc", "C5h", "Ks")},
            {nameof(Turn), I()},
            {nameof(River), I()},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5428")},
            {"Stack2", I("9330")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("360")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
        };


        // 3) Turn
        var turnState = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {nameof(Flop), I("Jc", "C5h", "Ks")},
            {nameof(Turn), I("C4h")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5328")},
            {"Stack2", I("9330")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("460")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
            {nameof(Decision), I("bar")}
        };

        // 4a) River a
        var riverState1 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {nameof(Flop), I("Jc", "C5h", "Ks")},
            {nameof(Turn), I("C4h")},
            {nameof(River), I("Kd")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5328")},
            {"Stack2", I("9330")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("460")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
            {nameof(Decision), I("bar")}
        };

        // 4b) River b
        var riverState2 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {nameof(Flop), I("Jc", "C5h", "Ks")},
            {nameof(Turn), I("C4h")},
            {nameof(River), I("Kd")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5028")},
            {"Stack2", I("9330")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("760")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
        };

        // 4c) River c
        var riverState3 = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I("C8c", "Kc")},
            {nameof(Flop), I("Jc", "C5h", "Ks")},
            {nameof(Turn), I("C4h")},
            {nameof(River), I("Kd")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5028")},
            {"Stack2", I("9030")},
            {"Stack3", I("9172")},
            {"Stack4", I("14171")},
            {"Stack5", I("7412")},
            {"Stack6", I("5817")},
            {nameof(Pot), I("1060")},
            {"Opponent1", I("cards")},
            {"Opponent2", I()},
            {"Opponent3", I()},
            {"Opponent4", I()},
            {"Opponent5", I()},
        };


        Dictionary<string, ReconResult>[] states =
        [
            prepreflopState,
            preflopState1,
            preflopState2,
            preflopState3,
            flopState1,
            flopState2,
            turnState,
            riverState1,
            riverState2,
            riverState3
        ];
        // Act: step through each phase
        // foreach (var state in states)
        // {
        //     poker.SetState(state);
        //     poker.Solve();
        // }

        // Expanded foreach for debugging
        poker.SetState(prepreflopState);
        poker.Solve();

        poker.SetState(preflopState1);
        poker.Solve();

        poker.SetState(preflopState2);
        poker.Solve();

        poker.SetState(preflopState3);
        poker.Solve();

        poker.SetState(flopState1);
        poker.Solve();

        poker.SetState(flopState2);
        poker.Solve();

        poker.SetState(turnState);
        poker.Solve();

        poker.SetState(riverState1);
        poker.Solve();

        poker.SetState(riverState2);
        poker.Solve();

        poker.SetState(riverState3);
        poker.Solve();
        
        var gameActions = poker.GameActions;
        var startingBets = poker.StartingBets;

        // Print them out for clarity
        _testOutputHelper.WriteLine("=== All Inferred Actions ===");
        foreach (var a in gameActions)
            _testOutputHelper.WriteLine(a.ToString());
        
        Assert.Equal(10, startingBets.Ante);
        Assert.Equal(50, startingBets.SmallBlind);
        Assert.Equal(100, startingBets.BigBlind);

        return;
        Assert.Collection(
            gameActions,
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(50, a.Amount);
                Assert.Equal(PokerPhase.Preflop, a.Phase);
            },
            a =>
            {
                Assert.Equal(2, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(100, a.Amount);
                Assert.Equal(PokerPhase.Preflop, a.Phase);
            },
            a =>
            {
                Assert.Equal(2, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(100, a.Amount);
                Assert.Equal(PokerPhase.Flop, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(100, a.Amount);
                Assert.Equal(PokerPhase.Turn, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(300, a.Amount);
                Assert.Equal(PokerPhase.River, a.Phase);
            },
            a =>
            {
                Assert.Equal(2, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(300, a.Amount);
                Assert.Equal(PokerPhase.River, a.Phase);
            }
        );
    }

    private ReconResult I(params string[] values)
    {
        return new ReconResult(
            new Rectangle(0, 0, 0, 0),
            values.ToList()
        );
    }
}