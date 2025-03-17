using System;
using System.Collections.Generic;
using System.Linq;
using PT.Algorithm.Model;
using PT.Interfaces;
using PT.Poker.Resolving;

namespace PT.Poker.Model
{
    public class WeightedCardSet : IWeightedEncounter, IRandomGenerated<SimulationParameters>
    {
        // My layout is always first
        private CardLayout[] _cardLayouts;
        private int _compareMyLayout;
        public double Weight { get; set; } = 1.0;

        public WeightedCardSet()
        {
        }

        public WeightedCardSet(CardLayout[] cardLayouts)
        {
            _cardLayouts = cardLayouts;
            Update();
        }

        private static readonly Random RandomGenerator = new();

        public void Generate(SimulationParameters parameters)
        {
            var usedCards = new byte[4, 13];

            var board = parameters.Board.ToList();
            var iboard = 0;
            for (; iboard < board.Count; iboard++)
            {
                var t = board[iboard];
                Set(usedCards, t);
            }

            if (parameters.MyLayout.Size > 2) throw new Exception("User layout can contain only <=2 cards");

            for (var i = 0; i < parameters.MyLayout.Size; i++)
            {
                Set(usedCards, parameters.MyLayout.Cards[i]);
            }

            for (; iboard < 5; iboard++)
            {
                var card = RandomCard(usedCards);
                board.Add(card);
            }

            var cardLayouts = new List<CardLayout>
            {
                GenerateRandomCards(usedCards, parameters.MyLayout, parameters.MyLayout.Size, 6, board)
            };

            var weightTotal = 0.0d;
            // Generate opponent hands with weighted probabilities
            for (var i = 1; i < parameters.NumOfPlayers; i++)
            {
                var (opponentHand, weight) = GenerateWeightedOpponentHand(usedCards, parameters.HandEquities);
                cardLayouts.Add(GenerateRandomCards(usedCards, opponentHand, opponentHand.Size, 6, board));
                weightTotal += weight;
            }

            Weight = parameters.NumOfPlayers > 1 ? weightTotal / (parameters.NumOfPlayers - 1) : 1.0d;

            _cardLayouts = cardLayouts.ToArray();

            Update();
        }

        private (CardLayout, double) GenerateWeightedOpponentHand(byte[,] usedCards, Dictionary<string[], double> handEquities)
        {
            var card1 = RandomCard(usedCards);
            var card2 = RandomCard(usedCards);
            var cardLayout = new CardLayout(new[] { card1, card2 });
            string[] cards = [card1.ToEString(), card2.ToEString()];
            if (!handEquities.TryGetValue(cards, out var weight))
            {
                if (!handEquities.TryGetValue([cards[1], cards[0]], out weight))
                {
                    return (cardLayout, 1.0d);
                }
            }

            return (cardLayout, weight);
        }

        private bool IsCardAvailable(byte[,] usedCards, Card card)
        {
            return usedCards[(int)card.CardColor, (int)card.CardType] == 0;
        }

        private Card ParseCard(string cardStr)
        {
            if (string.IsNullOrEmpty(cardStr) || cardStr.Length < 2)
                throw new ArgumentException("Invalid card format", nameof(cardStr));

            // Use the Card.FromEString method which is designed to parse the format produced by ToEString
            return Card.FromEString(cardStr);
        }

        private static void Set(byte[,] array, CardColor color, CardType type)
        {
            array[(int)color, (int)type] = 1;
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
                    var result = new Card((CardColor)color, (CardType)type);
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