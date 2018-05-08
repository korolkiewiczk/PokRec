using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Presenters;
using PT.Poker.Model;

namespace Game.RegionMatchers
{
    public abstract class CardsMatcher : IRegionMatcher<List<Card>>
    {
        private Board _board;

        public CardsMatcher(Board board)
        {
            _board = board;
        }

        public virtual RegionSpec GetRegionSpec()
        {
            return new RegionSpec
            {
                ClassesPath = ClassPath,
                Name = GetType().Name,
                Num = 1,
                Threshold = 60,
                AbandonThreshold = 50
            };
        }

        public List<Card> Match(GameResult result)
        {
            return result.Results.Select(x =>
            {
                var cardType = (CardType) Enum.Parse(typeof(CardType), x.Substring(0, x.Length - 1));
                var colorString = x.Substring(x.Length - 1);
                CardColor cardColor = CardColor.Clubs;
                switch (colorString)
                {
                    case "d":
                        cardColor = CardColor.Diamonds;
                        break;
                    case "s":
                        cardColor = CardColor.Spades;
                        break;
                    case "c":
                        cardColor = CardColor.Clubs;
                        break;
                    case "h":
                        cardColor = CardColor.Hearts;
                        break;
                }

                return new Card(cardColor, cardType);
            }).ToList();
        }

        public IResultPresenter<List<Card>> GetPresenter()
        {
            return new CardPresenter();
        }

        private string ClassPath => Classes.ClassPath(_board, "cards");
    }
}