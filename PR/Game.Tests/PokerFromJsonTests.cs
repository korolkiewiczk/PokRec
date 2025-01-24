using System.Text.Json;
using Common;
using Game.Games.TexasHoldem.Solving;
using Game.Games.TexasHoldem.Utils;
using Xunit.Abstractions;

namespace Game.Tests;

public class PokerFromJsonTests : PokerTestsBase
{
    public PokerFromJsonTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        
    }

    [Theory]
    [InlineData(6, "test_case1.json")]
    public void LoadLogFromJsonAndSolvesGame(int numPlayers, string testcaseFileName)
    {
        // Arrange
        var poker = new Poker(new Board
        {
            Settings =
            [
                new KeyValuePair<string, string>(nameof(PokerBoardSettingsParser.Players), numPlayers.ToString())
            ]
        });
        var jsonFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "TestCases",
            testcaseFileName
        );

        var lines = File.ReadAllLines(jsonFilePath);

        // Each line is a JSON object. We'll deserialize them into
        // Dictionary<string, ReconResult>.
        // If your actual code uses a simpler shape, adapt accordingly.
        var states = lines
            .Select(line =>
            {
                // First parse into Dictionary<string, List<string>>
                var dict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(line);

                // Then convert each List<string> into our ReconResult record
                // (the Rectangle is not important, so we'll pass default)
                var reconDict = dict!
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => new ReconResult(default, kvp.Value)
                    );

                return reconDict;
            })
            .ToArray();

        foreach (var state in states)
        {
            poker.SetState(state);
            poker.Solve();
        }

        var gameActions = poker.GameActions;
        var startingBets = poker.StartingBets;
        if (startingBets != null)
        {
            _testOutputHelper.WriteLine($"Ante={startingBets.Ante}, SmallBlind={startingBets.SmallBlind} BigBlind={startingBets.BigBlind}");
        }

        // Print them out for clarity
        _testOutputHelper.WriteLine("=== All Inferred Actions ===");
        foreach (var a in gameActions)
            _testOutputHelper.WriteLine(a.ToString());
    }
}