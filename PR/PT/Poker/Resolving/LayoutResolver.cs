using System;
using System.Collections.Generic;
using System.Linq;
using PT.Poker.Model;

namespace PT.Poker.Resolving
{
    public class LayoutResolver
    {
        private readonly CardLayout _layout;

        public CardLayout BestLayout { get; private set; }

        public PokerLayouts PokerLayout
        {
            get { return _pokerLayout; }
            private set { _pokerLayout = value; }
        }


        public LayoutResolver(CardLayout layout)
        {
            _layout = layout;
            Init();
        }


        private readonly int[] _bucketsType = new int[13];
        private readonly int[] _bucketsColor = new int[4];
        private PokerLayouts _pokerLayout = PokerLayouts.HighCard;

        private void Init()
        {
            GetBuckets();

            var bestFlush = GetFlush();

            int bestTwo;
            int secondTwo;
            int bestThree;
            int bestFour;
            int bestStraight;
            GetPokerLayouts(out bestTwo, out secondTwo, out bestThree, out bestFour, out bestStraight);

            PokerLayout = GetPockerLayout(bestFlush, bestTwo, secondTwo, bestThree, bestFour, bestStraight);

            BestLayout = GetBestLayout(PokerLayout, bestFlush, bestTwo, secondTwo, bestThree, bestFour, bestStraight);
        }

        private CardLayout GetBestLayout(PokerLayouts pokerLayout, int bestFlush, int bestTwo, int secondTwo, int bestThree, int bestFour, int bestStraight)
        {
            if (pokerLayout == PokerLayouts.None) return _layout;
            CardLayout newCardLayout = new CardLayout(new Card[5]);
            HashSet<CardType> usedCardTypes = new HashSet<CardType>();
            HashSet<Card> usedCards = new HashSet<Card>();
            int j = 0;
            for (int i = 0; i < _layout.Size; i++)
            {
                var card = _layout.Cards[i];
                int type = (int)card.CardType;
                int color = (int)card.CardColor;
                bool assign = false;
                int weight = 0;
                if (pokerLayout == PokerLayouts.Poker && bestStraight == type && bestFlush == color)
                {
                    assign = true;
                }
                else
                if (pokerLayout == PokerLayouts.FourOfKind && bestFour == type)
                {
                    assign = true;
                }
                else
                if (pokerLayout == PokerLayouts.FullHouse && (bestThree == type || bestTwo == type))
                {
                    if (bestThree == type)
                        weight = Card.WeightLevel2;
                    else if (bestTwo == type)
                        weight = Card.WeightLevel1;
                    assign = true;
                }
                else
                if (pokerLayout == PokerLayouts.Flush && bestFlush == color)
                {
                    assign = true;
                }
                else
                if (pokerLayout == PokerLayouts.Straight && ((bestStraight >= type && bestStraight < type + 5) || (bestStraight == 3 && type == (int)CardType.A)))
                {
                    if (!usedCardTypes.Contains((CardType) type))
                    {
                        assign = true;
                    }
                }
                else
                if (pokerLayout == PokerLayouts.ThreeOfKind && bestThree == type)
                {
                    assign = true;
                }
                else
                if (pokerLayout == PokerLayouts.TwoPair && (bestTwo == type || secondTwo == type))
                {
                    assign = true;
                }
                else
                if (pokerLayout == PokerLayouts.Pair && bestTwo == type)
                {
                    assign = true;
                }
                if (assign)
                {
                    if (j >= newCardLayout.Size)
                    {
                        var minCard = newCardLayout.Cards.Min();
                        if (minCard.CompareTo(card) < 0)
                        {
                            int ind = newCardLayout.Cards.ToList().IndexOf(minCard);

                            newCardLayout.Cards[ind] = card;
                            usedCardTypes.Add(card.CardType);
                            usedCards.Add(card);
                        }
                    }
                    else
                    {
                        newCardLayout.Cards[j++] = card;
                        usedCardTypes.Add(card.CardType);
                        usedCards.Add(card);
                    }
                }
            }
            var newArray = _layout.Cards.OrderByDescending(x => x).ToArray();
            for (int i = 0; i < newArray.Length && j < 5; i++)
            {
                if (!usedCards.Contains(newArray[i]))
                {
                    newCardLayout.Cards[j++] = newArray[i];
                    usedCardTypes.Add(newArray[i].CardType);
                }
            }
            if (j < 5) throw new Exception("Number of cards in new layout < 5");
            return newCardLayout;
        }

        private PokerLayouts GetPockerLayout(int bestFlush, int bestTwo, int secondTwo, int bestThree, int bestFour, int bestStraight)
        {
            if (Ok(bestFlush) && Ok(bestStraight))
                return PokerLayouts.Poker;

            if (Ok(bestFour))
                return PokerLayouts.FourOfKind;

            if (Ok(bestThree) && Ok(bestTwo))
                return PokerLayouts.FullHouse;

            if (Ok(bestFlush))
                return PokerLayouts.Flush;

            if (Ok(bestStraight))
                return PokerLayouts.Straight;

            if (Ok(bestThree))
                return PokerLayouts.ThreeOfKind;

            if (Ok(bestTwo) && Ok(secondTwo))
                return PokerLayouts.TwoPair;

            if (Ok(bestTwo))
                return PokerLayouts.Pair;

            return PokerLayouts.HighCard;
        }

        private bool Ok(int val)
        {
            return val >= 0;
        }

        private void GetBuckets()
        {
            foreach (var card in _layout.Cards)
            {
                ++_bucketsType[(int)card.CardType];
                ++_bucketsColor[(int)card.CardColor];
            }
        }

        private int GetFlush()
        {
            int bestFlush = -1;
            for (int i = 0; i < _bucketsColor.Length; i++)
            {
                int colorTimes = _bucketsColor[i];
                if (colorTimes >= 5)
                {
                    bestFlush = i;
                }
            }
            return bestFlush;
        }

        private void GetPokerLayouts(out int bestTwo, out int secondTwo, out int bestThree, out int bestFour, out int bestStraight)
        {
            bestTwo = -1;
            secondTwo = -1;
            bestThree = -1;
            bestFour = -1;
            bestStraight = -1;

            if (_layout.Size < 5) return;

            int straightCounter = -1;
            for (int i = 0; i < _bucketsType.Length; i++)
            {
                int typeTimes = _bucketsType[i];
                if (typeTimes == 2)
                {
                    if (bestTwo != -1)
                        secondTwo = bestTwo;
                    bestTwo = i;
                }

                else if (typeTimes == 3) bestThree = i;
                else if (typeTimes == 4) bestFour = i;

                if (typeTimes > 0 && straightCounter == -1)
                {
                    if (_bucketsType[12] > 0 && i == 0)
                        straightCounter = 2;
                    else
                        straightCounter = 1;
                }
                else if (typeTimes > 0 && straightCounter > 0)
                {
                    ++straightCounter;
                }
                else if (typeTimes <= 0)
                {
                    straightCounter = -1;
                }
                if (straightCounter >= 5)
                {
                    bestStraight = i;
                }
            }
        }
    }
}
