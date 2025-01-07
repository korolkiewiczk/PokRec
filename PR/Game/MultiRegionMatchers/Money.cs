using System.Collections.Generic;
using System.Linq;
using Common;

namespace Game.MultiRegionMatchers;

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
            decimal value = MoneyParser.ParseMoneyValue(rawText);
            if (value != 0)
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