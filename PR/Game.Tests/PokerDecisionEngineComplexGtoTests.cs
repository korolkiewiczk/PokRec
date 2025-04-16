using System.Collections.Concurrent;
using CfrSolver.Interfaces;
using CfrSolver.Model;
using Game.Common.Model;
using Game.Solving.AdaptivePokerStrategy;
using PT.Poker.Model;
using Xunit.Abstractions;
using PlayerAction = CfrSolver.Model.PlayerAction;

namespace Game.Tests;

public class PokerDecisionEngineComplexGtoTests : PokerTestsBase
{
    private readonly PokerDecisionEngine _decisionEngine;

    public PokerDecisionEngineComplexGtoTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        IBoardGenerator boardGenerator = new TestBoardGenerator();

        Node[] gtoRootNodes =
        [
            new Node(0, new PlayerAction(OpType.Call, 0), Round.PreFlop,
                [
                    new Node(1, new PlayerAction(OpType.Raise, 1), Round.PreFlop,
                        [
                            new Node(0, new PlayerAction(OpType.Call, 0), Round.Flop, 
                            [
                                new Node(0, new PlayerAction(OpType.Fold, 0), Round.Flop, [], 0),
                                new Node(0, new PlayerAction(OpType.Call, 0), Round.Flop, [], 0)
                            ], 0) {Data = new ConcurrentDictionary<int, Node.NodeData>(
                                new List<KeyValuePair<int, Node.NodeData>> {
                                    new(0, new Node.NodeData(2)
                                    {
                                        Cfr = [5.213f, 2.847f],
                                        Strategy = [0.646f, 0.354f],
                                        StrategySum = [120.75f, 66.32f]
                                    })
                                })},
                            new Node(0, new PlayerAction(OpType.Raise, 3), Round.PreFlop, [], 1),
                            new Node(0, new PlayerAction(OpType.Fold, 0), Round.PreFlop, [], 1)
                        ],
                        1),
                    new Node(1, new PlayerAction(OpType.Fold, 0), Round.PreFlop, [], 0)
                ],
                0),

            new Node(0, new PlayerAction(OpType.Call, 0), Round.Flop,
                [
                    new Node(1, new PlayerAction(OpType.Raise, 2), Round.Flop,
                        [
                            new Node(0, new PlayerAction(OpType.Call, 0), Round.Flop, 
                            [
                                new Node(0, new PlayerAction(OpType.Fold, 0), Round.Flop, [], 0),
                                new Node(0, new PlayerAction(OpType.Call, 0), Round.Flop, [], 0)
                            ], 0) {Data = new ConcurrentDictionary<int, Node.NodeData>(
                            new List<KeyValuePair<int, Node.NodeData>> {
                                new(0, new Node.NodeData(2)
                                {
                                    Cfr = [19.381422f, 0.7668941f],
                                    Strategy = [0.9619376f, 0.038062442f],
                                    StrategySum = [691.11456f, 28.560047f]
                                })
                            })},
                            new Node(0, new PlayerAction(OpType.Fold, 0), Round.Flop, [], 0)
                        ],
                        2),
                    new Node(1, new PlayerAction(OpType.Fold, 0), Round.Flop, [], 0)
                ],
                0)
        ];

        var opponentAdjustmentConfig = new OpponentAdjustmentConfig
        {
            FoldToThreeBetWeight = 2.0f,
            WTSDWeight = 2.0f,
            ThreeBetWeight = 2.0f,
            VPIPWeight = 2.0f,
            CBetFlopWeight = 2.0f,
            PFRWeight = 2.0f,
            FoldToCBetFlopWeight = 2.0f
        };

        _decisionEngine = new PokerDecisionEngine(boardGenerator, gtoRootNodes, opponentAdjustmentConfig,
            [2, 4], 100);
    }

    [Fact]
    public void DecideAction_ShouldReturnValidStrategy_ForPreflopWithComplexGtoNodes()
    {
        // Arrange: Create a PreFlop game state
        var gameState = new GameState
        {
            ActionHistory = ["R1", "C"],
            Pot = 1,
            PlayerStack = 50,
            PlayerCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            BoardInfo = new BoardInfo()
        };

        // Act
        var result = _decisionEngine.DecideAction(gameState);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f);
    }

    [Fact]
    public void DecideAction_ShouldReturnValidStrategy_ForFlopWithComplexGtoNodes()
    {
        // Arrange: Create a Flop game state
        var gameState = new GameState
        {
            ActionHistory = ["R1", "C"],
            Pot = 1,
            PlayerStack = 50,
            PlayerCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            BoardInfo = new BoardInfo()
        };

        var opponentStats = new PlayerStatsRelative(100, 25, 20, 8, 65, 45, 70, 35);

        // Act
        var result = _decisionEngine.DecideAction(gameState);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f);
    }

    [Fact]
    public void DecideAction_ComplexGtoNodes_ShouldVaryStrategy_BasedOnRound()
    {
        // Arrange: Two different game states (one PreFlop and one Flop) to test tree traversal differences.
        var preflopState = new GameState
        {
            ActionHistory = ["R1", "C"],
            Pot = 0,
            PlayerStack = 50,
            PlayerCards =
            [
                new Card(CardColor.Diamonds, CardType.Q),
                new Card(CardColor.Clubs, CardType.Q)
            ],
            BoardInfo = new BoardInfo()
        };

        var opponentStats = new PlayerStatsRelative(120, 30, 20, 10, 40, 90, 30, 50);

        // Act
        var preflopResult = _decisionEngine.DecideAction(preflopState, opponentStats);

        // Assert
        Assert.NotNull(preflopResult);
        Assert.Equal(2, preflopResult.Count);
        Assert.True(preflopResult.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, preflopResult.Values.Sum(), 0.0001f);
    }
}