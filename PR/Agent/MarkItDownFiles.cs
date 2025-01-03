using System.Text;
using Common;
using System.IO;
using System;
using PT.Poker.Model;
using Game.RegionMatchers;
using Game.MultiRegionMatchers;

namespace Agent;

public static class MarkItDownFiles
{
    public static void GenerateMarkItDownFiles()
    {
        const string classesTxt = Paths.Classes;
        const string regionsTxt = Paths.Regions;
        if (File.Exists(classesTxt) && File.Exists(regionsTxt))
        {
            return;
        }

        // adding classes
        StringBuilder classesContent = new StringBuilder();
        foreach (var cardType in Enum.GetValues(typeof(CardType)))
        {
            foreach (var cardColor in Enum.GetValues(typeof(CardColor)))
            {
                classesContent.AppendLine($"cards\\{cardType}{char.ToLower(cardColor.ToString()![0])}");
            }
        }

        classesContent.AppendLine("position\\btn");
        classesContent.AppendLine("opponent\\cards");

        File.WriteAllText(classesTxt, classesContent.ToString());

        // adding regions
        string[] regionsContent =
        [
            nameof(Flop),
            nameof(Turn),
            nameof(River),
            nameof(PlayerCards)
        ];

        var regionContents = new StringBuilder(string.Join("\r\n", regionsContent)).AppendLine();

        var maxPlayers = 10;
        for (int i = 0; i < maxPlayers; i++)
        {
            regionContents.AppendLine($"{nameof(Position)}{i + 1}");
        }

        for (int i = 0; i < maxPlayers; i++)
        {
            regionContents.AppendLine($"{nameof(Opponent)}{i + 1}");
        }

        for (int i = 0; i < maxPlayers; i++)
        {
            regionContents.AppendLine($"{nameof(Stack)}{i + 1}");
        }

        for (int i = 0; i < maxPlayers; i++)
        {
            regionContents.AppendLine($"{nameof(Nickname)}{i + 1}");
        }

        for (int i = 0; i < maxPlayers; i++)
        {
            regionContents.AppendLine($"{nameof(PlayerBet)}{i + 1}");
        }

        File.WriteAllText(regionsTxt, regionContents.ToString());
    }
}