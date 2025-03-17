using System;
using System.Collections.Generic;
using System.Linq;
using PT.Interfaces;

namespace PT.Poker.Model
{
    public abstract class BaseCardSet<T> : IEncounter, IRandomGenerated<T>
    {
        protected CardLayout[] _cardLayouts;
        protected int _compareMyLayout;
        protected static readonly Random RandomGenerator = new();

        protected BaseCardSet()
        {
        }

        protected BaseCardSet(CardLayout[] cardLayouts)
        {
            _cardLayouts = cardLayouts;
            Update();
        }

        public abstract void Generate(T arg);

        private static void Set(byte[,] array, CardColor color, CardType type)
        {
            array[(int)color, (int)type] = 1;
        }

        protected static void Set(byte[,] array, Card card)
        {
            Set(array, card.CardColor, card.CardType);
        }

        protected static Card RandomCard(byte[,] array)
        {
            var attempts = 1000;
            do
            {
                var color = RandomGenerator.Next(4);
                var type = RandomGenerator.Next(13);
                if (array[color, type] == 0)
                {
                    var result = new Card((CardColor)color, (CardType)type);
                    Set(array, result);
                    return result;
                }
            } while (--attempts > 0);

            throw new Exception("Unable to generate random card after 1000 attempts.");
        }

        protected CardLayout GenerateRandomCards(byte[,] array, CardLayout layout, int start, int end, List<Card> boardCards)
        {
            var size = end - start + 1;
            if (size <= 0) return new CardLayout(layout.Cards);

            if (layout == null)
            {
                layout = new CardLayout(new Card[size]);
            }
            else if (layout.Size < size)
            {
                var newCards = new Card[size + layout.Size];
                Array.Copy(layout.Cards, newCards, layout.Size);
                layout = new CardLayout(newCards);
            }

            var jBoard = 0;
            for (var i = start; i <= end; i++, jBoard++)
            {
                layout.Cards[i] = jBoard < boardCards.Count ? boardCards[jBoard] : RandomCard(array);
            }

            return layout;
        }

        protected void Update()
        {
            var myLayout = GetMyLayout();
            _compareMyLayout = 1;

            for (var i = 1; i < _cardLayouts.Length; i++)
            {
                var comparison = myLayout.CompareTo(_cardLayouts[i]);
                if (comparison < 0)
                {
                    _compareMyLayout = -1;
                    break;
                }

                if (comparison == 0)
                {
                    _compareMyLayout = Math.Min(_compareMyLayout, 0);
                }
            }
        }

        public bool IsWinning => _compareMyLayout == 1;
        public bool IsLoosing => _compareMyLayout == -1;
        public CardLayout[] CardLayouts => _cardLayouts;

        private CardLayout GetMyLayout() => _cardLayouts[0];
    }
}