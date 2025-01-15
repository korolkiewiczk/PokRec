using System;
using System.Collections.Generic;

namespace PT.Poker.Model
{
    public readonly struct Card : IComparable<Card>, IEquatable<Card>
    {
        public CardType CardType { get; }

        public CardColor CardColor { get; }

        public Card(CardColor cardColor, CardType cardType)
        {
            CardColor = cardColor;
            CardType = cardType;
        }

        public override int GetHashCode()
        {
            return (int) CardType * 4 + (int) CardColor;
        }

        public int CompareTo(Card other)
        {
            return CardType.CompareTo(other.CardType);
        }

        public int Power => 1 << (int) CardType;

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
        
        public string ToCString()
        {
            var type = (int) CardType + 2 > 10 ? CardType.ToString() : ((int) CardType + 2).ToString();
            return type + CardColor;
        }
        
        public string ToEString()
        {
            var type = CardType.ToString();
            return type + char.ToLower(CardColor.ToString()[0]);
        }
        
        public static Card FromEString(string eString)
        {
            if (string.IsNullOrEmpty(eString) || eString.Length < 2)
                throw new ArgumentException("Invalid eString format", nameof(eString));

            var typeStr = eString[..^1]; // All but last char
            var colorChar = char.ToUpper(eString[^1]); // Last char

            if (!Enum.TryParse<CardType>(typeStr, out var cardType))
                throw new ArgumentException($"Invalid card type: {typeStr}", nameof(eString));

            var colorMap = new Dictionary<char, CardColor>
            {
                {'C', CardColor.Clubs},
                {'D', CardColor.Diamonds}, 
                {'H', CardColor.Hearts},
                {'S', CardColor.Spades}
            };

            if (!colorMap.TryGetValue(colorChar, out var cardColor))
                throw new ArgumentException($"Invalid card color: {colorChar}", nameof(eString));

            return new Card(cardColor, cardType);
        }


        public bool Equals(Card other)
        {
            return CardType == other.CardType && CardColor == other.CardColor;
        }

        public override bool Equals(object obj)
        {
            return obj is Card other && Equals(other);
        }
    }
}