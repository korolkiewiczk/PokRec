using Common;
using Game.Common.Utils;

namespace Game.Common.MultiRegionMatchers;

public abstract class Money : MultiTextNumericMatcher
{
    protected Money(int seats) : base(seats)
    {
    }

    public override List<decimal?> Match(IEnumerable<ReconResult> results)
    {
        List<decimal?> money = new List<decimal?>();
        foreach (var result in results)
        {
            string rawText = result.Result;
            decimal? value = MoneyParser.ParseMoneyValue(rawText);
            if (value != null)
            {
                money.Add(value);
            }
            else
            {
                money.Add(null);
            }
        }

        return money;
    }
}