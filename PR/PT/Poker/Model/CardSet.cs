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

        private static readonly Random _random = new Random();

        public CardSet()
        {
        }

        public CardSet(CardLayout[] cardLayouts)
        {
            _cardLayouts = cardLayouts;
            Update();
        }

        public void Generate(RandomSetDefinition arg)
        {
            byte[,] usedCards = new byte[4, 13];

            List<Card> board = arg.Board.ToList();
            int iboard = 0;
            for (; iboard < board.Count; iboard++)
            {
                Card t = board[iboard];
                Set(usedCards, t);
            }

            if (arg.MyLayout.Size > 2) throw new Exception("User layout can contain only <=2 cards");

            for (int i = 0; i < arg.MyLayout.Size; i++)
            {
                Set(usedCards, arg.MyLayout.Cards[i]);
            }

            for (; iboard < 5; iboard++)
            {
                Card card = RandomCard(usedCards);
                board.Add(card);
            }

            var cardLayouts = new List<CardLayout>
            {
                GenerateRandomCards(usedCards, arg.MyLayout, arg.MyLayout.Size, 6, board)
            };

            for (int i = 1; i < arg.NumOfPlayers; i++)
            {
                cardLayouts.Add(GenerateRandomCards(usedCards, null, 0, 6, board));
            }

            _cardLayouts = cardLayouts.ToArray();

            
            Update();
        }

        private void Set(byte[,] array, CardColor color, CardType type)
        {
            array[(int) color, (int) type] = 1;
        }

        private void Set(byte[,] array, Card card)
        {
            Set(array, card.CardColor, card.CardType);
        }

        private Card RandomCard(byte[,] array)
        {
            int k = 1000;
            do
            {
                int color = _random.Next(4);
                int type = _random.Next(13);
                if (array[color, type] == 0)
                {
                    Card result=new Card((CardColor)color, (CardType)type);
                    Set(array, result);
                    return result;
                }
            } while ((--k)>0);
            throw new Exception("RANDOMCARD k<0");
        }

        private CardLayout GenerateRandomCards(byte[,] array, CardLayout layout, int start, int end, List<Card> boardCards)
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
            int jBoard = 0;
            for (int i = start; i <= end; i++, jBoard++)
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
            for (int i = 1; i < _cardLayouts.Length; i++)
            {
                var comparison = myLayout.CompareTo(_cardLayouts[i]);
                if (comparison < 0)
                {
                    _compareMyLayout = -1;
                    break;
                }
                if (comparison == 0)
                {
                    _compareMyLayout = Math.Min(_compareMyLayout,0);
                }
            }
        }

        public bool IsWinning
        {
            get { return _compareMyLayout == 1; }
        }

        public bool IsLoosing
        {
            get { return _compareMyLayout == -1; }
        }

        private CardLayout GetMyLayout()
        {
            return _cardLayouts[0];
        }
    }
}
