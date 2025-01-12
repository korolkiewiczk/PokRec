using System;
using System.Collections.Generic;
using System.Linq;
using PT.Interfaces;

namespace PT.Poker.Model
{
    public class CardSet : IEncounter, IRandomGenerated<RandomSetDefinition>
    {
        //my layout is always first
        private CardLayout[] _cardLayouts;
        private int _compareMyLayout;

        public CardSet()
        {
        }

        public CardSet(CardLayout[] cardLayouts)
        {
            _cardLayouts = cardLayouts;
            Update();
        }

        private static readonly Random RandomGenerator = new();

        public void Generate(RandomSetDefinition arg)
        {
            var usedCards = new byte[4, 13];

            var board = arg.Board.ToList();
            var iboard = 0;
            for (; iboard < board.Count; iboard++)
            {
                var t = board[iboard];
                Set(usedCards, t);
            }

            if (arg.MyLayout.Size > 2) throw new Exception("User layout can contain only <=2 cards");

            for (var i = 0; i < arg.MyLayout.Size; i++)
            {
                Set(usedCards, arg.MyLayout.Cards[i]);
            }

            for (; iboard < 5; iboard++)
            {
                var card = RandomCard(usedCards);
                board.Add(card);
            }

            var cardLayouts = new List<CardLayout>
            {
                GenerateRandomCards(usedCards, arg.MyLayout, arg.MyLayout.Size, 6, board)
            };

            for (var i = 1; i < arg.NumOfPlayers; i++)
            {
                cardLayouts.Add(GenerateRandomCards(usedCards, null, 0, 6, board));
            }

            _cardLayouts = cardLayouts.ToArray();

            Update();
        }

        private static void Set(byte[,] array, CardColor color, CardType type)
        {
            array[(int) color, (int) type] = 1;
        }

        private static void Set(byte[,] array, Card card)
        {
            Set(array, card.CardColor, card.CardType);
        }

        private static Card RandomCard(byte[,] array)
        {
            var k = 1000;
            do
            {
                var color = RandomGenerator.Next(4);
                var type = RandomGenerator.Next(13);
                if (array[color, type] == 0)
                {
                    var result = new Card((CardColor) color, (CardType) type);
                    Set(array, result);
                    return result;
                }
            } while (--k > 0);

            throw new Exception("RANDOMCARD k<0");
        }

        private CardLayout GenerateRandomCards(byte[,] array, CardLayout layout, int start, int end,
            List<Card> boardCards)
        {
            var size = end - start + 1;
            if (size <= 0) return new CardLayout(layout.Cards);
            if (layout == null)
            {
                layout = new CardLayout(new Card[size]);
            }
            else
            {
                if (layout.Size < size)
                {
                    var newCards = new Card[size + layout.Size];
                    Array.Copy(layout.Cards, newCards, layout.Size);
                    layout = new CardLayout(newCards);
                }
            }

            var jBoard = 0;
            for (var i = start; i <= end; i++, jBoard++)
            {
                if (jBoard < boardCards.Count)
                {
                    layout.Cards[i] = boardCards[jBoard];
                }
                else
                {
                    layout.Cards[i] = RandomCard(array);
                }
            }

            return layout;
        }

        private void Update()
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

        private CardLayout GetMyLayout()
        {
            return _cardLayouts[0];
        }
    }
}