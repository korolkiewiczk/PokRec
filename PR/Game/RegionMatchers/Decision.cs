using Common;
using Game.Interfaces;
using Game.Presenters;

namespace Game.RegionMatchers
{
    public class Decision : IRegionMatcher<bool>
    {
        private readonly Board _board;

        public Decision(Board board)
        {
            _board = board;
        }

        public RegionSpec GetRegionSpec()
        {
            return new RegionSpec
            {
                ClassesPath = ClassPath,
                Name = GetType().Name,
                Num = 1,
                Threshold = 90
            };
        }

        public bool Match(ReconResult result)
        {
            if (result?.Results == null)
            {
                return false;
            }
            
            // If we got any result at all, it means the decision indicator was detected
            return result.Results.Count > 0;
        }

        public IResultPresenter GetPresenter()
        {
            return new DecisionPresenter();
        }

        private string ClassPath => Classes.ClassPath(_board, "decision");
    }
} 