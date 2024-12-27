using System.Diagnostics;
using PT.Algorithm;
using PT.Algorithm.Model;
using PT.Interfaces;
using PT.Poker.Model;
using Xunit.Abstractions;

namespace PT.Tests;

public class MonteCarloTests
{
    private readonly ITestOutputHelper _output;
    private const int n = 1000000;


    public MonteCarloTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Solve_AllWinning_Returns_ProbabilityOfWinning_1_And_ProbabilityOfLosing_0()
    {
        // Arrange
        var monteCarlo = new MonteCarlo<AlwaysWinningEncounter, object>(n, null!);
        Stopwatch stopwatch = new();

        // Act
        stopwatch.Start();
        MonteCarloResult result = monteCarlo.Solve();
        stopwatch.Stop();

        // Assert
        Assert.Equal(1.0, result.Better, 5);
        Assert.Equal(0.0, result.Smaller, 5);
        _output.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void Solve_AllLosing_Returns_ProbabilityOfWinning_0_And_ProbabilityOfLosing_1()
    {
        // Arrange
        var monteCarlo = new MonteCarlo<AlwaysLosingEncounter, object>(n, null!);
        Stopwatch stopwatch = new();

        // Act
        stopwatch.Start();
        MonteCarloResult result = monteCarlo.Solve();
        stopwatch.Stop();

        // Assert
        Assert.Equal(0.0, result.Better, 5);
        Assert.Equal(1.0, result.Smaller, 5);
        _output.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void Solve_MixedResults_Returns_Approximately_HalfWinningAndHalfLosing()
    {
        // Arrange
        var monteCarlo = new MonteCarlo<MixedEncounter, object>(n, null!);
        Stopwatch stopwatch = new();

        // Act
        stopwatch.Start();
        MonteCarloResult result = monteCarlo.Solve();
        stopwatch.Stop();

        // Since we alternate wins and losses, we expect about half.
        // ProbabilityOfWinning ~ 0.5, ProbabilityOfLoosing ~ 0.5
        Assert.InRange(result.Better, 0.4, 0.6);
        Assert.InRange(result.Smaller, 0.4, 0.6);
        _output.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void Solve_WithCardSet_ShouldCalculateProbabilities()
    {
        // Arrange
        int numIterations = 1000;
        var cardSetDefinition = new RandomSetDefinition
        {
            Board =
            [
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Diamonds, CardType.C2),
                new Card(CardColor.Spades, CardType.C3)
            ],
            MyLayout = new CardLayout([
                new Card(CardColor.Clubs, CardType.C4),
                new Card(CardColor.Hearts, CardType.C5)
            ]),
            NumOfPlayers = 3
        };

        var monteCarlo = new MonteCarlo<CardSet, RandomSetDefinition>(numIterations, cardSetDefinition);

        Stopwatch stopwatch = new();
        // Act
        stopwatch.Start();
        MonteCarloResult result = monteCarlo.Solve();
        stopwatch.Stop();

        // Assert
        Assert.InRange(result.Better, 0.0, 1.0); // Winning probability should be between 0 and 1
        Assert.InRange(result.Smaller, 0.0, 1.0); // Losing probability should be between 0 and 1
        Assert.InRange(result.Exact, 0.0, 1.0); // Tie probability should be between 0 and 1

        // Ensure probabilities sum to approximately 1
        double totalProbability = result.Better + result.Smaller + result.Exact;
        Assert.InRange(totalProbability, 0.999, 1.001);
        
        _output.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void Solve_WithSingleIteration_ShouldReturnDeterministicResults()
    {
        // Arrange
        int numIterations = 1;
        var cardSetDefinition = new RandomSetDefinition
        {
            Board = [new Card(CardColor.Hearts, CardType.A)],
            MyLayout = new CardLayout([
                new Card(CardColor.Clubs, CardType.C4)
            ]),
            NumOfPlayers = 2
        };

        var monteCarlo = new MonteCarlo<CardSet, RandomSetDefinition>(numIterations, cardSetDefinition);

        Stopwatch stopwatch = new();
        // Act
        stopwatch.Start();
        MonteCarloResult result = monteCarlo.Solve();
        stopwatch.Stop();
        // Assert
        // Since it's deterministic with 1 iteration, the probabilities are 0 or 1
        Assert.True(result.Better == 1.0 || result.Better == 0.0);
        Assert.True(result.Smaller == 1.0 || result.Smaller == 0.0);
        Assert.True(result.Exact == 1.0 || result.Exact == 0.0);

        double totalProbability = result.Better + result.Smaller + result.Exact;
        Assert.Equal(1.0, totalProbability, 5);
        
        _output.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void Solve_WithStrongHand_ShouldHaveHighWinningProbability()
    {
        // Arrange
        int numIterations = 5000; // High number of iterations for statistical significance
        var cardSetDefinition = new RandomSetDefinition
        {
            Board =
            [
                new Card(CardColor.Hearts, CardType.C2),
                new Card(CardColor.Diamonds, CardType.C3),
                new Card(CardColor.Spades, CardType.C4)
            ],
            MyLayout = new CardLayout([
                new Card(CardColor.Hearts, CardType.A), // Strong hand: two Aces
                new Card(CardColor.Clubs, CardType.A)
            ]),
            NumOfPlayers = 3 // 3 players in total
        };

        var monteCarlo = new MonteCarlo<CardSet, RandomSetDefinition>(numIterations, cardSetDefinition);

        Stopwatch sw = new();
        // Act
        sw.Start();
        MonteCarloResult result = monteCarlo.Solve();
        sw.Stop();
        
        // Assert
        // Since we have a very strong hand, we expect a high probability of winning
        Assert.InRange(result.Better, 0.7, 0.95); // Winning probability should be high
        Assert.InRange(result.Smaller, 0.0, 0.3); // Losing probability should be low
        Assert.InRange(result.Exact, 0.0, 0.1); // Tie probability should be minimal

        double totalProbability = result.Better + result.Smaller + result.Exact;
        Assert.InRange(totalProbability, 0.999, 1.001); // Ensure probabilities sum to 1
        
        _output.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");
    }


    public class AlwaysLosingEncounter : IEncounter, IRandomGenerated<object>
    {
        public bool IsWinning => false;
        public bool IsLoosing => true;

        public void Generate(object arg)
        {
            // Simulating a small computation by performing an integer sum loop
            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += i % 3;
            }

            // To ensure the sum is not optimized away
            if (sum < 0) throw new InvalidOperationException();
        }
    }

    public class AlwaysWinningEncounter : IEncounter, IRandomGenerated<object>
    {
        public bool IsWinning => true;
        public bool IsLoosing => false;

        public void Generate(object arg)
        {
            // Simulating computation by performing a math operation
            double result = 0.0;
            for (int i = 1; i <= 10; i++)
            {
                result += Math.Sqrt(i);
            }

            // To ensure the result is not optimized away
            if (result < 0) throw new InvalidOperationException();
        }
    }

    public class MixedEncounter : IEncounter, IRandomGenerated<object>
    {
        private static int _callCount = 0;

        public bool IsWinning { get; private set; }
        public bool IsLoosing { get; private set; }

        public void Generate(object arg)
        {
            // For the sake of testing, let's say that half the time it's winning, 
            // and half the time it's losing.
            // We'll just alternate each call.
            _callCount++;
            if (_callCount % 2 == 0)
            {
                IsWinning = true;
                IsLoosing = false;
            }
            else
            {
                IsWinning = false;
                IsLoosing = true;
            }
        }
    }
}