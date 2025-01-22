using System.Drawing;
using System.Globalization;
using Common;
using Game.MultiRegionMatchers;
using Game.RegionMatchers;

namespace Game.Tests;

public static class DictArrayExt
{
    public static Dictionary<string, ReconResult>[] AddState(this Dictionary<string, ReconResult>[] dictArray, 
        string[] playerCards, string[] flopCards, string[] turnCards, string[] riverCards, int buttonPos,
        decimal[] stackDiffs, decimal pot, bool[] opponentsInGame, bool isDecision)
    {
        // Create a new state dictionary
        var newState = new Dictionary<string, ReconResult>
        {
            // Add player cards
            [nameof(PlayerCards)] = I(playerCards),
            [nameof(Flop)] = I(flopCards),
            [nameof(Turn)] = I(turnCards),
            [nameof(River)] = I(riverCards)
        };

        // Add button positions
        for (var i = 1; i <= opponentsInGame.Length + 1; i++)
        {
            newState[$"Position{i}"] = i == buttonPos ? I("btn") : I();
        }

        // Update stacks
        for (int i = 0; i < stackDiffs.Length; i++)
        {
            // Extract current stack value and subtract stackDiffs
            var currentStack = dictArray.LastOrDefault()?[$"Stack{i + 1}"]?.Result ?? "0";
            decimal stackValue = decimal.TryParse(currentStack, out var parsedValue) ? parsedValue : 0;
            newState[$"Stack{i + 1}"] = I((stackValue - stackDiffs[i]).ToString(CultureInfo.InvariantCulture));
        }

        // Add pot
        newState[nameof(Pot)] = I(pot.ToString(CultureInfo.InvariantCulture));

        // Update opponents in game
        for (int i = 0; i < opponentsInGame.Length; i++)
        {
            newState[$"Opponent{i + 1}"] = opponentsInGame[i] ? I("cards") : I();
        }

        // Add decision if required
        if (isDecision)
        {
            newState[nameof(isDecision)] = I("bar");
        }

        // Add the new state to the array
        var updatedArray = dictArray.Append(newState).ToArray();
        return updatedArray;
    }
    
    private static ReconResult I(params string[] values)
    {
        return new ReconResult(
            new Rectangle(0, 0, 0, 0),
            values.ToList()
        );
    }
}