using System;
using System.Drawing;

namespace PT.Poker.Model;

public static class PokerLayoutsExtensions
{
    public static string ToDisplayString(this PokerLayouts layout)
    {
        return layout switch
        {
            PokerLayouts.None => "None",
            PokerLayouts.HighCard => "High Card",
            PokerLayouts.Pair => "Pair",
            PokerLayouts.TwoPair => "Two Pair",
            PokerLayouts.ThreeOfKind => "Three of a Kind",
            PokerLayouts.Straight => "Straight",
            PokerLayouts.Flush => "Flush",
            PokerLayouts.FullHouse => "Full House",
            PokerLayouts.FourOfKind => "Four of a Kind",
            PokerLayouts.Poker => "Poker",
            _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, null)
        };
    }

    public static Color ToColor(this PokerLayouts layout)
    {
        return layout switch
        {
            PokerLayouts.Poker => Color.Gold,
            PokerLayouts.FourOfKind => Color.Red,
            PokerLayouts.FullHouse => Color.Blue,
            PokerLayouts.Flush => Color.Green,
            PokerLayouts.Straight => Color.Orange,
            PokerLayouts.ThreeOfKind => Color.Purple,
            PokerLayouts.TwoPair => Color.Brown,
            PokerLayouts.Pair => Color.Gray,
            PokerLayouts.HighCard => Color.Black,
            _ => Color.Black
        };
    }
}