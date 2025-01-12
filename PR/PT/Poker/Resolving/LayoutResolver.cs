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

        public PokerLayouts PokerLayout { get; private set; } = PokerLayouts.HighCard;


        public LayoutResolver(CardLayout layout)
        {
            _layout = layout;
            Init();
        }

        private readonly int[] _bucketsType = new int[13];
        private readonly int[] _bucketsColor = new int[4];

        private void Init()
        {
            GetBuckets();

            var bestFlush = GetFlush();

            var (bestTwo, secondTwo, bestThree, bestFour, bestStraight) = GetPokerLayouts();

            PokerLayout = GetPokerLayout(bestFlush, bestTwo, secondTwo, bestThree, bestFour, bestStraight);

            BestLayout = GetBestLayout(PokerLayout, bestFlush, bestTwo, secondTwo, bestThree, bestFour, bestStraight);
        }

        private CardLayout GetBestLayout(PokerLayouts pokerLayout, int bestFlush, int bestTwo, int secondTwo,
            int bestThree, int bestFour, int bestStraight)
        {
            if (pokerLayout == PokerLayouts.None) return _layout;

            var newCardLayout = new CardLayout(new Card[Math.Min(5, _layout.Size)]);
            var usedCardTypes = new HashSet<CardType>();
            var usedCards = new HashSet<Card>();
            var j = 0;

            AssignCardsToLayout(pokerLayout, bestFlush, bestTwo, secondTwo, bestThree, bestFour, bestStraight,
                newCardLayout, usedCardTypes, usedCards, ref j);

            if (j >= 5) return newCardLayout;

            AddRemainingCardsToLayout(newCardLayout, usedCards, ref j);

            return newCardLayout;
        }

        private void AssignCardsToLayout(PokerLayouts pokerLayout, int bestFlush, int bestTwo, int secondTwo,
            int bestThree, int bestFour, int bestStraight, CardLayout newCardLayout, HashSet<CardType> usedCardTypes,
            HashSet<Card> usedCards, ref int j)
        {
            foreach (var card in _layout.Cards)
            {
                var type = (int) card.CardType;
                var color = (int) card.CardColor;
                var assign = ShouldAssignCardToLayout(pokerLayout, bestFlush, bestTwo, secondTwo, bestThree, bestFour,
                    bestStraight, usedCardTypes, type, color, card);

                if (!assign) continue;
                if (j >= newCardLayout.Size)
                {
                    ReplaceMinCardIfNecessary(newCardLayout, card);
                }
                else
                {
                    newCardLayout.Cards[j++] = card;
                }

                usedCardTypes.Add(card.CardType);
                usedCards.Add(card);
            }
        }

        private static void ReplaceMinCardIfNecessary(CardLayout newCardLayout, Card card)
        {
            var minCard = newCardLayout.Cards.Min();
            if (minCard.CompareTo(card) >= 0) return;
            var ind = Array.IndexOf(newCardLayout.Cards, minCard);
            newCardLayout.Cards[ind] = card;
        }

        private static bool ShouldAssignCardToLayout(PokerLayouts pokerLayout, int bestFlush, int bestTwo,
            int secondTwo, int bestThree, int bestFour, int bestStraight, HashSet<CardType> usedCardTypes, int type,
            int color, Card card)
        {
            return (pokerLayout, bestStraight == type, bestFlush == color, bestFour == type,
                    bestThree == type, bestTwo == type, secondTwo == type) switch
                {
                    (PokerLayouts.Poker, true, true, _, _, _, _) => true,
                    (PokerLayouts.FourOfKind, _, _, true, _, _, _) => true,
                    (PokerLayouts.FullHouse, _, _, _, true, true, _) => true,
                    (PokerLayouts.Flush, _, true, _, _, _, _) => true,
                    (PokerLayouts.Straight, true, _, _, _, _, _) => true,
                    (PokerLayouts.Straight, _, _, _, _, _, _) when bestStraight == 3 && type == (int) CardType.A &&
                                                                   !usedCardTypes.Contains(card.CardType) => true,
                    (PokerLayouts.ThreeOfKind, _, _, _, true, _, _) => true,
                    (PokerLayouts.TwoPair, _, _, _, _, true, true) => true,
                    (PokerLayouts.Pair, _, _, _, _, true, _) => true,
                    _ => false
                };
        }

        private void AddRemainingCardsToLayout(CardLayout newCardLayout, HashSet<Card> usedCards, ref int j)
        {
            foreach (var card in _layout.Cards.OrderByDescending(x => x))
            {
                if (j >= 5) break;
                if (usedCards.Contains(card)) continue;
                newCardLayout.Cards[j++] = card;
            }
        }

        private PokerLayouts GetPokerLayout(int bestFlush, int bestTwo, int secondTwo, int bestThree, int bestFour,
            int bestStraight)
        {
            return (IsValid(bestFlush), IsValid(bestStraight), IsValid(bestFour), IsValid(bestThree), IsValid(bestTwo),
                    IsValid(secondTwo)) switch
                {
                    (true, true, _, _, _, _) => PokerLayouts.Poker,
                    (_, _, true, _, _, _) => PokerLayouts.FourOfKind,
                    (_, _, _, true, true, _) => PokerLayouts.FullHouse,
                    (true, _, _, _, _, _) => PokerLayouts.Flush,
                    (_, true, _, _, _, _) => PokerLayouts.Straight,
                    (_, _, _, true, _, _) => PokerLayouts.ThreeOfKind,
                    (_, _, _, _, true, true) => PokerLayouts.TwoPair,
                    (_, _, _, _, true, _) => PokerLayouts.Pair,
                    _ => PokerLayouts.HighCard
                };
        }

        private bool IsValid(int val)
        {
            return val >= 0;
        }

        private void GetBuckets()
        {
            foreach (var card in _layout.Cards)
            {
                ++_bucketsType[(int) card.CardType];
                ++_bucketsColor[(int) card.CardColor];
            }
        }

        private int GetFlush()
        {
            var bestFlush = -1;
            for (var i = 0; i < _bucketsColor.Length; i++)
            {
                var colorTimes = _bucketsColor[i];
                if (colorTimes >= 5)
                {
                    bestFlush = i;
                }
            }

            return bestFlush;
        }

        private Tuple<int, int, int, int, int> GetPokerLayouts()
        {
            int secondTwo;
            int bestThree;
            int bestFour;
            int bestStraight;
            var bestTwo = secondTwo = bestThree = bestFour = bestStraight = -1;
            if (_layout.Size < 5)
            {
                for (int i = 0; i < _bucketsType.Length; i++)
                {
                    if (_bucketsType[i] == 2)
                    {
                        bestTwo = i;
                        break;
                    }
                }

                return Tuple.Create(bestTwo, secondTwo, bestThree, bestFour, bestStraight);
            }

            var straightCounter = -1;
            for (var i = 0; i < _bucketsType.Length; i++)
            {
                var typeTimes = _bucketsType[i];
                switch (typeTimes)
                {
                    case 2:
                        secondTwo = bestTwo != -1 ? bestTwo : secondTwo;
                        bestTwo = i;
                        break;
                    case 3:
                        bestThree = i;
                        break;
                    case 4:
                        bestFour = i;
                        break;
                }

                if (typeTimes > 0)
                {
                    straightCounter = straightCounter == -1
                        ? (i == 0 && _bucketsType[12] > 0 ? 2 : 1)
                        : straightCounter + 1;
                }
                else
                {
                    straightCounter = -1;
                }

                if (straightCounter >= 5) bestStraight = i;
            }

            return Tuple.Create(bestTwo, secondTwo, bestThree, bestFour, bestStraight);
        }
    }
}