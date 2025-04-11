using CfrSolver.Interfaces;
using CfrSolver.Model;
using Game.Common.Model;
using Game.Solving.AdaptivePokerStrategy;
using PT.Poker.Model;
using Xunit.Abstractions;

namespace Game.Tests;

public class PokerDecisionEngineTests : PokerTestsBase
{
    private readonly PokerDecisionEngine _decisionEngine;

    public PokerDecisionEngineTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        IBoardGenerator boardGenerator =
            // Initialize test dependencies
            new TestBoardGenerator();
        Node[] gtoRootNodes =
        [
            new Node(0, new CfrSolver.Model.PlayerAction(OpType.Call, 0), Round.PreFlop, [], 0)
        ]; // Simplified GTO node for testing
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
    public void DecideAction_ShouldReturnValidStrategy_ForPreflopHeadsUp()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 0,
            ActionHistory = ["R1", "R1"], // SB vs BB
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

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count); // Should have probabilities for Fold, Call, and Raise
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f); // Probabilities should sum to 1
    }

    [Fact]
    public void DecideAction_ShouldHandleFlopAction_WithBoardCards()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 1,
            ActionHistory = ["R1", "C", "C"], // Preflop action
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            Board = new BoardInfo
            {
                Hand = 1 // Simplified hand strength for testing
            }
        };

        var opponentStats = new PlayerStatsRelative(100, 25, 20, 8, 65, 45, 70, 35);

        // Act
        var result = _decisionEngine.DecideAction(gameState, opponentStats);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count);
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f);
    }

    [Fact]
    public void DecideAction_ShouldAdjustStrategy_ForOpponentTendencies()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 0,
            ActionHistory = ["C"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            Board = new BoardInfo()
        };

        // Test with a tight opponent (low VPIP, high fold to 3bet)
        var tightOpponentStats = new PlayerStatsRelative(100, 15, 12, 5, 80, 35, 60, 45);

        // Test with a loose opponent (high VPIP, low fold to 3bet)
        var looseOpponentStats = new PlayerStatsRelative(100, 5, 25, 12, 45, 55, 75, 25);

        // Act
        var tightResult = _decisionEngine.DecideAction(gameState, tightOpponentStats);
        var looseResult = _decisionEngine.DecideAction(gameState, looseOpponentStats);

        // Assert
        Assert.NotNull(tightResult);
        Assert.NotNull(looseResult);

        // Strategy should be more aggressive against tight opponent
        Assert.True(tightResult["R2"] > looseResult["R2"]);
        // Strategy should be more passive against loose opponent
        Assert.True(looseResult["C"] > tightResult["C"]);
    }

    [Fact]
    public void DecideAction_ShouldIncreaseAggression_WhenOpponentFoldsOftenToThreeBet()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 0,
            ActionHistory = ["R1"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Diamonds, CardType.Q),
                new Card(CardColor.Clubs, CardType.Q)
            ],
            Board = new BoardInfo()
        };

        var opponentFoldsOftenStats = new PlayerStatsRelative(150, 20, 15, 8, 90, 50, 40, 40);
        var opponentRarelyFoldsStats = new PlayerStatsRelative(150, 20, 15, 8, 10, 50, 40, 40);

        // Act
        var foldsOftenResult = _decisionEngine.DecideAction(gameState, opponentFoldsOftenStats);
        var rarelyFoldsResult = _decisionEngine.DecideAction(gameState, opponentRarelyFoldsStats);

        // Assert
        Assert.NotNull(foldsOftenResult);
        Assert.NotNull(rarelyFoldsResult);

        // More aggression when opponent folds often
        Assert.True(foldsOftenResult["R2"] > rarelyFoldsResult["R2"]);
    }

    [Fact]
    public void DecideAction_ShouldIncreaseCalling_WhenOpponentFrequentlyCBetFlop()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 1,
            ActionHistory = ["R1", "C1", "C1"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Spades, CardType.J),
                new Card(CardColor.Hearts, CardType.C10)
            ],
            Board = new BoardInfo()
        };

        var frequentCBetOpponentStats = new PlayerStatsRelative(120, 30, 20, 10, 40, 90, 30, 50);
        var rareCBetOpponentStats = new PlayerStatsRelative(120, 30, 20, 10, 40, 20, 30, 50);

        // Act
        var frequentCBetResult = _decisionEngine.DecideAction(gameState, frequentCBetOpponentStats);
        var rareCBetResult = _decisionEngine.DecideAction(gameState, rareCBetOpponentStats);

        // Assert
        Assert.NotNull(frequentCBetResult);
        Assert.NotNull(rareCBetResult);

        // Higher calling probability against frequent C-Better
        Assert.True(frequentCBetResult["C"] > rareCBetResult["C"]);
    }

    [Fact]
    public void DecideAction_ShouldIncreaseFolding_WhenOpponentHasHighPFRAndFoldToCBet()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 0,
            ActionHistory = ["R1", "R1"],
            Pay = 0,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Clubs, CardType.C9),
                new Card(CardColor.Hearts, CardType.C8)
            ],
            Board = new BoardInfo()
        };

        var aggressiveButFoldsToPressureStats = new PlayerStatsRelative(200, 30, 40, 15, 50, 60, 85, 35);
        var passiveStickyOpponentStats = new PlayerStatsRelative(200, 30, 10, 15, 50, 60, 10, 35);

        // Act
        var foldsToPressureResult = _decisionEngine.DecideAction(gameState, aggressiveButFoldsToPressureStats);
        var stickyOpponentResult = _decisionEngine.DecideAction(gameState, passiveStickyOpponentStats);

        // Assert
        Assert.NotNull(foldsToPressureResult);
        Assert.NotNull(stickyOpponentResult);

        // Increased folding probability against aggressive but fold-prone opponent
        Assert.True(foldsToPressureResult["F"] > stickyOpponentResult["F"]);
    }

    [Fact]
    public void GetEquityBasedStrategy_ShouldReturnValidStrategy_WhenGTOFails()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 0,
            ActionHistory = ["R1", "R1"],
            Pay = 100,
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

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count); // Should have probabilities for Fold, Call, and Raise
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f); // Probabilities should sum to 1
    }

    [Fact]
    public void GetEquityBasedStrategy_ShouldReturnWeightedDistribution_ForWeakCards()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 0,
            ActionHistory = ["R1", "R1"],
            Pay = 2,
            PlayerStack = 50,
            PlayerHoleCards =
            [
                new Card(CardColor.Clubs, CardType.C2),
                new Card(CardColor.Diamonds, CardType.C3)
            ], // Weak hand
            Board = new BoardInfo(),
        };

        var opponentStats = new PlayerStatsRelative(100, 25, 20, 8, 65, 45, 70, 35);

        // Act
        var result = _decisionEngine.DecideAction(gameState, opponentStats);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count);
        Assert.Collection(result.Values,
            p => Assert.Equal(0.3f, p, 0.1f),
            p => Assert.Equal(0.5f, p, 0.1f),
            p => Assert.Equal(0.2f, p, 0.1f),
            p => Assert.Equal(0.0f, p, 0.1f),
            p => Assert.Equal(0.0f, p, 0.1f),
            p => Assert.Equal(0.0f, p, 0.1f),
            p => Assert.Equal(0.0f, p, 0.1f)
            );
    }

    [Fact]
    public void GetEquityBasedStrategy_ShouldHandleMixedPositiveAndNegativeEVs()
    {
        // Arrange
        var gameState = new GameState
        {
            CurrentPlayer = 0,
            Round = 0,
            ActionHistory = ["R1", "R1", "C", "C"],
            Pay = -50,
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

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count);
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f);

        // Verify that actions with higher EV have higher probabilities
        // In this case, fold should have the highest probability due to negative pay
        Assert.True(result["F"] > result["C"]);
        Assert.True(result["F"] > result["R2"]);
    }

    private class TestBoardGenerator : IBoardGenerator
    {
        public BoardInfo GenerateBoardAbstraction()
        {
            return new BoardInfo
            {
                Hand = 1,
                OppHand = 0,
                WinningPlayer = 0
            };
        }

        public BoardInfo GenerateBoardAbstraction(Card[] playerHoleCards, Card[] flopCards, Card[] turnCards,
            Card[] riverCards)
        {
            return new BoardInfo
            {
                Hand = 1,
                OppHand = 0,
                WinningPlayer = 0
            };
        }

        public BoardInfo GenerateBoardAbstractionRandom(Card[] playerHoleCards, Card[] flopCards, Card[] turnCards,
            Card[] riverCards, Card[] opponentHoleCards)
        {
            return new BoardInfo
            {
                Hand = 1,
                OppHand = 0,
                WinningPlayer = 0
            };
        }
    }
}