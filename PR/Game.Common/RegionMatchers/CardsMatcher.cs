using Common;
using Game.Interfaces;
using PT.Poker.Model;

namespace Game.Common.RegionMatchers
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

        private string ClassPath => Classes.ClassPath(_board, "cards");
    }
}