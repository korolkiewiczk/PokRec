using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Interfaces;
using Game.Presenters;

namespace Game.MultiRegionMatchers
{
    public class Stack : TextMatcher<List<decimal?>>
    {
        private readonly int _seats;

        public Stack(int seats)
        {
            _seats = seats;
        }

        public static decimal ParseMoneyValue(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return 0;
            }
            string cleanText = rawText.Replace("$", "").Replace(",", "").Trim();
            var parsed = decimal.TryParse(cleanText, out decimal value);
            return parsed ? value : 0;
        }

        public override IResultPresenter GetPresenter()
        {
            return new StackPresenter();
        }

        public override List<decimal?> Match(IEnumerable<ReconResult> results)
        {
            List<decimal?> money = [];
            foreach (var result in results)
            {
                if (result?.Results != null)
                {
                    string rawText = result.Results.FirstOrDefault();
                    decimal value = ParseMoneyValue(rawText);
                    if (value != 0)
                    {
                        money.Add(value);
                    }
                    else
                    {
                        money.Add(null);
                    }
                }
            }

            return money;
        }

        public override IEnumerable<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> specs = new List<RegionSpec>();
            for (int i = 0; i < _seats; i++)
            {
                var spec = GetSingleRegionSpec(i);
                specs.Add(spec);
            }

            return specs;
        }
    }
}