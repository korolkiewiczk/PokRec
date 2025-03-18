using System;
using System.Collections.Generic;
using System.Linq;
using PT.Interfaces;

namespace PT.Poker.Model
{
    public class WeightedCardSet : BaseCardSet<SimulationParameters>, IWeightedEncounter
    {
        public double Weight { get; set; } = 1.0;

        public WeightedCardSet()
        {
        }

        public WeightedCardSet(CardLayout[] cardLayouts)
        {
            _cardLayouts = cardLayouts;
            Update();
        }

        public override void Generate(SimulationParameters parameters)
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

        private (CardLayout, double) GenerateWeightedOpponentHand(byte[,] usedCards,
            Dictionary<string, double> handEquities)
        {
            var card1 = RandomCard(usedCards);
            var card2 = RandomCard(usedCards);
            var cardLayout = new CardLayout(new[] {card1, card2});
            if (!handEquities.TryGetValue($"{card1.ToEString()},{card2.ToEString()}", out var weight))
            {
                if (!handEquities.TryGetValue($"{card2.ToEString()},{card1.ToEString()}", out weight))
                {
                    return (cardLayout, 1.0d);
                }
            }

            return (cardLayout, weight);
        }
    }
} 