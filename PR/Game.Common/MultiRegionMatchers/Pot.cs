using Common;
using Game.Common.Utils;

namespace Game.Common.MultiRegionMatchers;

public class Pot : TextMatcher<decimal?>
{
    public override decimal? Match(IEnumerable<ReconResult> results)
    {
        return MoneyParser.ParseMoneyValue(results?.FirstOrDefault()?.Result ?? "");
    }

    public override IEnumerable<RegionSpec> GetRegionSpecs()
    {
        yield return GetSingleRegionSpec(-1);
    }
}