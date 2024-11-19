using Common;
using Game.Presenters;

namespace Game.RegionMatchers
{
    public class TextMatcher : IRegionMatcher<string>
    {
        private readonly string _classPath;
        private readonly int _threshold;

        public TextMatcher(string classPath, int threshold = 60)
        {
            _classPath = classPath;
            _threshold = threshold;
        }

        public RegionSpec GetRegionSpec()
        {
            return new RegionSpec
            {
                ClassesPath = _classPath,
                Name = GetType().Name,
                Num = 1,
                Threshold = _threshold
            };
        }

        public string Match(ReconResult result)
        {
            return string.Join(" ", result.Results);
        }

        public IResultPresenter<string> GetPresenter()
        {
            return new TextPresenter();
        }
    }
} 