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

        // Create at least 2 complex GTO root nodes with nested children.
        Node[] gtoRootNodes =
        [
            // Complex GTO tree for PreFlop decisions
            new Node(0, new PlayerAction(OpType.Call, 0), Round.PreFlop,
                [
                    // Option 1: A Raise branch with two follow-up actions.
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
                            // Nested branch: another raise option.
                            new Node(0, new PlayerAction(OpType.Raise, 3), Round.PreFlop, [], 1),
                            // Nested branch: a fold option after raising.
                            new Node(0, new PlayerAction(OpType.Fold, 0), Round.PreFlop, [], 1)
                        ],
                        1),
                    // Option 2: A direct fold action.
                    new Node(1, new PlayerAction(OpType.Fold, 0), Round.PreFlop, [], 0)
                ],
                0),

            // Complex GTO tree for Flop decisions
            new Node(0, new PlayerAction(OpType.Call, 0), Round.Flop,
                [
                    // Option 1: A Raise branch on the flop.
                    new Node(1, new PlayerAction(OpType.Raise, 2), Round.Flop,
                        [
                            // Nested branch: call after raising.
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
                            // Nested branch: fold after raising.
                            new Node(0, new PlayerAction(OpType.Fold, 0), Round.Flop, [], 0)
                        ],
                        2),
                    // Option 2: A fold branch on the flop.
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
            [2, 4, 6, 8], 100);
    }

    [Fact]
    public void DecideAction_ShouldReturnValidStrategy_ForPreflopWithComplexGtoNodes()
    {
        // Arrange: Create a PreFlop game state
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = (int) Round.PreFlop,
            ActionHistory = ["R1", "C"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            Board = new BoardInfo()
        };

        var opponentStats = new PlayerStatsRelative(100, 25, 20, 8, 65, 45, 70, 35);

        // Act
        var result = _decisionEngine.DecideAction(gameState, opponentStats);

        // Assert: Verify that the strategy is valid (contains probabilities for each action that sum to 1)
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
            CurrentPlayer = 0,
            Round = (int) Round.Flop,
            ActionHistory = ["R1", "C1", "C1"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            Board = new BoardInfo
            {
                Hand = 0 // simplified board info for testing purposes
            }
        };

        var opponentStats = new PlayerStatsRelative(100, 25, 20, 8, 65, 45, 70, 35);

        // Act
        var result = _decisionEngine.DecideAction(gameState, opponentStats);

        // Assert: Verify that the returned strategy is valid
        Assert.NotNull(result);
        Assert.Equal(7, result.Count);
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f);
    }

    [Fact]
    public void DecideAction_ComplexGtoNodes_ShouldVaryStrategy_BasedOnRound()
    {
        // Arrange: Two different game states (one PreFlop and one Flop) to test tree traversal differences.
        var preflopState = new GameState
        {
            CurrentPlayer = 0,
            Round = (int) Round.PreFlop,
            ActionHistory = ["R1", "C"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Diamonds, CardType.Q),
                new Card(CardColor.Clubs, CardType.Q)
            ],
            Board = new BoardInfo()
        };

        var flopState = new GameState
        {
            CurrentPlayer = 0,
            Round = (int) Round.Flop,
            ActionHistory = ["R1", "C1", "C1"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Spades, CardType.J),
                new Card(CardColor.Hearts, CardType.C10)
            ],
            Board = new BoardInfo
            {
                Hand = 2
            }
        };

        var opponentStats = new PlayerStatsRelative(120, 30, 20, 10, 40, 90, 30, 50);

        // Act
        var preflopResult = _decisionEngine.DecideAction(preflopState, opponentStats);
        var flopResult = _decisionEngine.DecideAction(flopState, opponentStats);

        // Assert: Verify that the strategies are different for different game rounds.
        Assert.NotNull(preflopResult);
        Assert.NotNull(flopResult);
        Assert.NotEqual(preflopResult, flopResult);
    }
}