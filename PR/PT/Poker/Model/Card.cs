using System;
using System.Collections.Generic;

namespace PT.Poker.Model
{
    public struct Card : IComparable<Card>
    {
        private readonly CardType _cardType;
        private readonly CardColor _cardColor;
        private readonly int _weight;

        public static readonly int WeightLevel1 = 1 << 13;
        public static readonly int WeightLevel2 = (1 << 13)*4;

        public CardType CardType
        {
            get { return _cardType; }
        }

        public CardColor CardColor
        {
            get { return _cardColor; }
        }

        public int Weight
        {
            get { return _weight; }
        }

        public Card(CardColor cardColor, CardType cardType, int weight = 0)
        {
            _cardColor = cardColor;
            _cardType = cardType;
            _weight = weight;
        }

        public override int GetHashCode()
        {
            return (int)CardType * 4 + (int)CardColor;
        }

        public int CompareTo(Card other)
        {
            return CardType.CompareTo(other.CardType);
        }

        public int Power
        {
            get { return (1 << ((int) CardType))*(1 + Weight); }
        }

        public override string ToString()
        {
            string type = ((int)CardType + 2) > 10 ? CardType.ToString() : ((int)CardType + 2).ToString();
            Dictionary<CardColor, string> dict = new Dictionary<CardColor, string>()
            {
                {CardColor.Clubs, "♣"},
                {CardColor.Diamonds, "♦"},
                {CardColor.Hearts, "♥"},
                {CardColor.Spades, "♠"},
            };
            return type + dict[CardColor];
        }
    }
}
