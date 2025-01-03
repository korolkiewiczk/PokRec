using System.Collections.Generic;
using System.Linq;
using Common;

namespace Game.MultiRegionMatchers;

public abstract class Money : MultiTextNumericMatcher
{
    protected Money(int seats) : base(seats)
    {
    }

    public static decimal ParseMoneyValue(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return 0;
        }

        string cleanText = new string(rawText.Where(c => char.IsDigit(c) || c == '.').ToArray());
        var parsed = decimal.TryParse(cleanText, out decimal value);
        return parsed ? value : 0;
    }

    public override List<decimal?> Match(IEnumerable<ReconResult> results)
    {
        List<decimal?> money = new List<decimal?>();
        foreach (var result in results)
        {
            string rawText = result.Result;
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

        return money;
    }
}