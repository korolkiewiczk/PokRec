using System;
using System.Collections.Generic;

namespace PT.Poker.Model
{
    public readonly struct Card : IComparable<Card>
    {
        public CardType CardType { get; }

        public CardColor CardColor { get; }

        private int Weight { get; }

        public Card(CardColor cardColor, CardType cardType, int weight = 0)
        {
            CardColor = cardColor;
            CardType = cardType;
            Weight = weight;
        }

        public override int GetHashCode()
        {
            return (int) CardType * 4 + (int) CardColor;
        }

        public int CompareTo(Card other)
        {
            return CardType.CompareTo(other.CardType);
        }

        public int Power => (1 << (int) CardType) * (1 + Weight);

        public override string ToString()
        {
            var type = (int) CardType + 2 > 10 ? CardType.ToString() : ((int) CardType + 2).ToString();
            var dict = new Dictionary<CardColor, string>
            {
                {CardColor.Clubs, "♣"},
                {CardColor.Diamonds, "♦"},
                {CardColor.Hearts, "♥"},
                {CardColor.Spades, "♠"}
            };
            return type + dict[CardColor];
        }
    }
}