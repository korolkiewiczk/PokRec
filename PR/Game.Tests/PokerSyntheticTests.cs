using Common;
using Game.Games.TexasHoldem.Model;
using Game.Games.TexasHoldem.Solving;
using Game.Games.TexasHoldem.Utils;
using Game.MultiRegionMatchers;
using Game.RegionMatchers;
using Xunit.Abstractions;

namespace Game.Tests;

public class PokerSyntheticTests : PokerTestsBase
{
    public PokerSyntheticTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void Player_D_Solve_ShouldInferActionsProperly_WhenSteppingThroughPhases()
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
        var prepreflopState = new Dictionary<string, ReconResult>()
        {
            {nameof(PlayerCards), I()},
            {nameof(Flop), I("C6c")},
            {nameof(Turn), I("C8c")},
            {nameof(River), I("C3c")},
            {"Position1", I()},
            {"Position2", I()},
            {"Position3", I()},
            {"Position4", I()},
            {"Position5", I()},
            {"Position6", I("btn")},
            {"Stack1", I("5538")},
            {"Stack2", I("9540")},
            {"Stack3", I("9182")},
            {"Stack4", I("14181")},
            {"Stack5", I("7422")},
            {"Stack6", I("5827")},
            {nameof(Pot), I("460")}
        };

        var states = ((Dictionary<string, ReconResult>[]) [prepreflopState])
            .AddState(["C8s", "C7c"], [],
                [], [], 1, [10, 60, 110, 10, 10, 10], 210,
                [true, true, true, true, true], false)
            .AddState(["C8s", "C7c"], [],
                [], [], 1, [0, 0, 0, 0, 0, 0], 210,
                [false, true, false, false, false], true)
            .AddState(["C8s", "C7c"], [],
                [], [], 1, [200, 0, 0, 0, 0, 0], 410,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], [],
                [], [], 1, [0, 0, 300, 0, 0, 0], 710,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], [],
                [], [], 1, [0, 0, 0, 0, 0, 0], 710,
                [false, true, false, false, false], true)
            .AddState(["C8s", "C7c"], [],
                [], [], 1, [100, 0, 0, 0, 0, 0], 810,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                [], [], 1, [0, 0, 0, 0, 0, 0], 810,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                [], [], 1, [0, 0, 100, 0, 0, 0], 910,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                [], [], 1, [0, 0, 0, 0, 0, 0], 910,
                [false, true, false, false, false], true)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], [], 1, [100, 0, 0, 0, 0, 0], 1010,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], [], 1, [200, 0, 0, 0, 0, 0], 1210,
                [false, true, false, false, false], true)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], [], 1, [0, 0, 200, 0, 0, 0], 1410,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], ["Jc"], 1, [0, 0, 0, 0, 0, 0], 1410,
                [false, true, false, false, false], true)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], ["Jc"], 1, [500, 0, 0, 0, 0, 0], 1910,
                [false, true, false, false, false], true)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], ["Jc"], 1, [0, 0, 8472, 0, 0, 0], 1910 + 8472,
                [false, true, false, false, false], false)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], ["Jc"], 1, [0, 0, 0, 0, 0, 0], 1910 + 8472,
                [false, true, false, false, false], true)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], ["Jc"], 1, [4428, 0, 0, 0, 0, 0], 1910 + 8472 + 4428,
                [false, true, false, false, false], false);

        // Act: step through each phase
        foreach (var state in states)
        {
            poker.SetState(state);
            poker.Solve();
        }

        var gameActions = poker.GameActions;
        var startingBets = poker.StartingBets;

        // Print them out for clarity
        _testOutputHelper.WriteLine("=== All Inferred Actions ===");
        foreach (var a in gameActions)
            _testOutputHelper.WriteLine(a.ToString());

        Assert.Equal(10, startingBets.Ante);
        Assert.Equal(50, startingBets.SmallBlind);
        Assert.Equal(100, startingBets.BigBlind);

        Assert.Collection(
            gameActions,
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(200, a.Amount);
                Assert.Equal(PokerPhase.Preflop, a.Phase);
            },
            a =>
            {
                Assert.Equal(3, a.PlayerIndex);
                Assert.Equal(PokerActionType.Raise, a.ActionType);
                Assert.Equal(300, a.Amount);
                Assert.Equal(PokerPhase.Preflop, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Call, a.ActionType);
                Assert.Equal(100, a.Amount);
                Assert.Equal(PokerPhase.Preflop, a.Phase);
            },
            a =>
            {
                Assert.Equal(3, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(100, a.Amount);
                Assert.Equal(PokerPhase.Flop, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Call, a.ActionType);
                Assert.Equal(100, a.Amount);
                Assert.Equal(PokerPhase.Flop, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(200, a.Amount);
                Assert.Equal(PokerPhase.Turn, a.Phase);
            },
            a =>
            {
                Assert.Equal(3, a.PlayerIndex);
                Assert.Equal(PokerActionType.Call, a.ActionType);
                Assert.Equal(200, a.Amount);
                Assert.Equal(PokerPhase.Turn, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(500, a.Amount);
                Assert.Equal(PokerPhase.River, a.Phase);
            },
            a =>
            {
                Assert.Equal(3, a.PlayerIndex);
                Assert.Equal(PokerActionType.AllIn, a.ActionType);
                Assert.Equal(8472, a.Amount);
                Assert.Equal(PokerPhase.River, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.AllIn, a.ActionType);
                Assert.Equal(4428, a.Amount);
                Assert.Equal(PokerPhase.River, a.Phase);
            }
        );
    }

    [Fact]
    public void Player_LowestStack_AllIn_ShouldInferActionsProperly_AtTurn()
    {
        // Arrange
        var poker = new Poker(new Board
        {
            Settings =
            [
                new KeyValuePair<string, string>(nameof(PokerBoardSettingsParser.Players), "3")
            ]
        });

        var prepreflopState = new Dictionary<string, ReconResult>
        {
            {nameof(PlayerCards), I()},
            {nameof(Flop), I("C6c")},
            {nameof(Turn), I("C8c")},
            {nameof(River), I("C3c")},
            {"Position1", I()},
            {"Position2", I("btn")},
            {"Position3", I()},
            {"Stack1", I("1500")},
            {"Stack2", I("5000")},
            {"Stack3", I("8000")},
            {nameof(Pot), I("200")}
        };

        var states = ((Dictionary<string, ReconResult>[]) [prepreflopState])
            .AddState(["C8s", "C7c"], [],
                [], [], 3, [60, 110, 10], 180,
                [true, true], false)
            .AddState(["C8s", "C7c"], [],
                [], [], 3, [0, 0, 50], 230,
                [true, true], false)
            .AddState(["C8s", "C7c"], [],
                [], [], 3, [50, 0, 0], 280,
                [true, true], true)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                [], [], 3, [0, 0, 0], 280,
                [true, true], false)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                [], [], 3, [0, 0, 0], 280,
                [true, true], true)
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], [], 3, [0, 0, 0], 280,
                [true, true], false);
            states = states.AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], [], 3, [1390, 0, 0], 280+1390,
                [true, true], true) // Player with lowest stack goes all in
            .AddState(["C8s", "C7c"], ["C8c", "Ks", "Qs"],
                ["C2h"], [], 3, [0, 1390, 0], 280+1390,
                [true, false], false); // Player with highest stack calls, other folds

        // Act: step through each phase
        foreach (var state in states)
        {
            poker.SetState(state);
            poker.Solve();
        }

        var gameActions = poker.GameActions;

        // Print them out for clarity
        _testOutputHelper.WriteLine("=== All Inferred Actions ===");
        foreach (var a in gameActions)
            _testOutputHelper.WriteLine(a.ToString());
        
        Assert.Equal(10, poker.StartingBets.Ante);
        Assert.Equal(50, poker.StartingBets.SmallBlind);
        Assert.Equal(100, poker.StartingBets.BigBlind);

        Assert.Collection(
            gameActions,
            a =>
            {
                Assert.Equal(3, a.PlayerIndex);
                Assert.Equal(PokerActionType.Bet, a.ActionType);
                Assert.Equal(50, a.Amount);
                Assert.Equal(PokerPhase.Preflop, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.Call, a.ActionType);
                Assert.Equal(50, a.Amount);
                Assert.Equal(PokerPhase.Preflop, a.Phase);
            },
            a =>
            {
                Assert.Equal(1, a.PlayerIndex);
                Assert.Equal(PokerActionType.AllIn, a.ActionType);
                Assert.Equal(1390, a.Amount);
                Assert.Equal(PokerPhase.Turn, a.Phase);
            },
            a =>
            {
                Assert.Equal(2, a.PlayerIndex);
                Assert.Equal(PokerActionType.Call, a.ActionType);
                Assert.Equal(1390, a.Amount);
                Assert.Equal(PokerPhase.Turn, a.Phase);
            }
        );
    }
}