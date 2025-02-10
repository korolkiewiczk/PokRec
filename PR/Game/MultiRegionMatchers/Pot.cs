using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Interfaces;
using Game.Presenters;

namespace Game.MultiRegionMatchers;

public class Pot : TextMatcher<decimal?>
{
    public override decimal? Match(IEnumerable<ReconResult> results)
    {
        return MoneyParser.ParseMoneyValue(results?.FirstOrDefault()?.Result ?? "");
    }

    public override IResultPresenter GetPresenter()
    {
        return new StackPresenter();
    }

    public override IEnumerable<RegionSpec> GetRegionSpecs()
    {
        yield return GetSingleRegionSpec(-1);
    }
}