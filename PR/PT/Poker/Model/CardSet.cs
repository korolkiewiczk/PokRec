using System;
using System.Collections.Generic;
using System.Linq;
using PT.Interfaces;

namespace PT.Poker.Model
{
    public class CardSet : BaseCardSet<RandomSetDefinition>
    {
        public CardSet()
        {
        }

        public CardSet(CardLayout[] cardLayouts)
        {
            _cardLayouts = cardLayouts;
            Update();
        }

        public override void Generate(RandomSetDefinition arg)
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
    }
}