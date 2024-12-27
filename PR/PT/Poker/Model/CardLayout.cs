using System;
using PT.Interfaces;
using PT.Poker.Resolving;

namespace PT.Poker.Model
{
    public class CardLayout : IComparable, IMarkable
    {
        public Card[] Cards { get; }

        public CardLayout(Card[] cards)
        {
            Cards = cards;
        }

        public int CompareTo(object other)
        {
            return GetMark().CompareTo(((IMarkable) other)!.GetMark());
        }

        public IMark GetMark()
        {
            var resolver = new LayoutResolver(this);

            var cardPowerResolver = new CardPowerResolver(resolver.BestLayout);
            var layoutPower = cardPowerResolver.Resolve();

            return new PokerMark(resolver.PokerLayout, layoutPower);
        }

        public override string ToString()
        {
#if !DEBUG
            return string.Join(" ", Cards).Replace("♠","S").Replace("♣","C").Replace("♥","H").Replace("♦","D");
#else
            return string.Join(" ", Cards);
#endif
        }

        public int Size => Cards.Length;

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var card in Cards)
            {
                hash = hash * 23 + card.GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (CardLayout) obj;
            if (Cards.Length != other.Cards.Length)
            {
                return false;
            }

            for (int i = 0; i < Cards.Length; i++)
            {
                if (!Cards[i].Equals(other.Cards[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}