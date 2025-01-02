using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Interfaces;
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
                Threshold = 90
            };
        }

        public List<Card> Match(ReconResult result)
        {
            if (result?.Results == null)
            {
                return [];
            }
            return result.Results.Select(x =>
            {
                var cardType = (CardType) Enum.Parse(typeof(CardType), x.Substring(0, x.Length - 1));
                var colorString = x.Substring(x.Length - 1);
                var cardColor = colorString switch
                {
                    "d" => CardColor.Diamonds,
                    "s" => CardColor.Spades,
                    "c" => CardColor.Clubs,
                    "h" => CardColor.Hearts,
                    _ => CardColor.Clubs
                };

                return new Card(cardColor, cardType);
            }).ToList();
        }

        public IResultPresenter GetPresenter()
        {
            return new CardPresenter();
        }

        private string ClassPath => Classes.ClassPath(_board, "cards");
    }
}