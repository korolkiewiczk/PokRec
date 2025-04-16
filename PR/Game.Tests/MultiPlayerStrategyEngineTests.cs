using CfrSolver.Interfaces;
using CfrSolver.Model;
using Game.Common.Model;
using Game.Solving.AdaptivePokerStrategy;
using PT.Poker.Model;
using Xunit.Abstractions;

namespace Game.Tests;

public class MultiPlayerStrategyEngineTests : PokerTestsBase
{
    private readonly PokerDecisionEngine _headsUpEngine;
    private readonly MultiPlayerScalingConfig _scalingConfig;

    public MultiPlayerStrategyEngineTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        IBoardGenerator boardGenerator = new TestBoardGenerator();
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

        _headsUpEngine = new PokerDecisionEngine(boardGenerator, gtoRootNodes, opponentAdjustmentConfig, [2, 4, 6, 8], 100);
        _scalingConfig = new MultiPlayerScalingConfig
        {
            CompositeWeight = 0.5f,
            PairwiseWeight = 0.5f,
            AggressiveScalingCoefficient = 0.1f
        };
    }

    [Fact]
    public void ShouldFallbackToHeadsUp_WithSingleOpponent()
    {
        // Arrange
        var gameState = new GameState
        {
            ActionHistory = ["R1", "R1"],
            Pot = 100,
            PlayerCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            BoardInfo = new BoardInfo()
        };

        var opponentStats = new PlayerStatsRelative(100, 25, 20, 8, 65, 45, 70, 35);
        var mpState = new MultiPlayerGameState
        {
            BaseGameState = gameState,
            OpponentStats = [opponentStats]
        };

        var multiPlayerEngine = new MultiPlayerStrategyEngine(_headsUpEngine, _scalingConfig);

        // Act
        var result = multiPlayerEngine.DecideActionMultiPlayer(mpState);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count);
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f);
    }

    [Fact]
    public void ShouldBlendStrategies_WithMultipleOpponents()
    {
        // Arrange
        var gameState = new GameState
        {
            ActionHistory = ["R1", "R1"],
            Pot = 50,
            PlayerCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            BoardInfo = new BoardInfo(),
            PlayerStack = 10
        };

        // Create two opponents with different playing styles
        var tightOpponent = new PlayerStatsRelative(100, 15, 12, 5, 80, 35, 60, 45);
        var looseOpponent = new PlayerStatsRelative(100, 5, 25, 12, 45, 55, 75, 25);

        var mpState = new MultiPlayerGameState
        {
            BaseGameState = gameState,
            OpponentStats = [tightOpponent, looseOpponent]
        };

        var multiPlayerEngine = new MultiPlayerStrategyEngine(_headsUpEngine, _scalingConfig);

        // Act
        var result = multiPlayerEngine.DecideActionMultiPlayer(mpState);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Count);
        Assert.True(result.Values.All(p => p >= 0 && p <= 1));
        Assert.Equal(1.0f, result.Values.Sum(), 0.0001f);

        // Verify that aggressive actions are scaled down in multiway pots
        var headsUpResult = _headsUpEngine.DecideAction(gameState, tightOpponent);
        Assert.True(result["R2"] < headsUpResult["R2"]);
    }

    [Fact]
    public void ShouldHandleEmptyOpponentList()
    {
        // Arrange
        var gameState = new GameState
        {
            ActionHistory = ["R1", "R1"],
            Pot = 100,
            PlayerCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            BoardInfo = new BoardInfo()
        };

        var mpState = new MultiPlayerGameState
        {
            BaseGameState = gameState,
            OpponentStats = []
        };

        var multiPlayerEngine = new MultiPlayerStrategyEngine(_headsUpEngine, _scalingConfig);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => multiPlayerEngine.DecideActionMultiPlayer(mpState));
    }

    [Fact]
    public void ShouldScaleAggression_WithMoreOpponents()
    {
        // Arrange
        var gameState = new GameState
        {
            ActionHistory = ["R1", "R1"],
            Pot = 100,
            PlayerCards =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Spades, CardType.K)
            ],
            BoardInfo = new BoardInfo()
        };

        var opponentStats = new PlayerStatsRelative(100, 25, 20, 8, 65, 45, 70, 35);

        // Create states with different numbers of opponents
        var twoOpponentState = new MultiPlayerGameState
        {
            BaseGameState = gameState,
            OpponentStats = [opponentStats, opponentStats]
        };

        var threeOpponentState = new MultiPlayerGameState
        {
            BaseGameState = gameState,
            OpponentStats = [opponentStats, opponentStats, opponentStats]
        };

        var multiPlayerEngine = new MultiPlayerStrategyEngine(_headsUpEngine, _scalingConfig);

        // Act
        var twoOpponentResult = multiPlayerEngine.DecideActionMultiPlayer(twoOpponentState);
        var threeOpponentResult = multiPlayerEngine.DecideActionMultiPlayer(threeOpponentState);

        // Assert
        Assert.True(threeOpponentResult["R2"] < twoOpponentResult["R2"]);
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