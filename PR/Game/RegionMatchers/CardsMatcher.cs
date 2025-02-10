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

            return result.Results.OrderBy(x => x).Select(Card.FromEString).ToList();
        }

        public IResultPresenter GetPresenter()
        {
            return new CardPresenter();
        }

        private string ClassPath => Classes.ClassPath(_board, "cards");
    }
}