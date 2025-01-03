using System.Collections.Generic;
using System.Linq;
using Common;

namespace Game.MultiRegionMatchers;

public abstract class Money : TextMatcher<List<decimal?>>
{
    private readonly int _seats;

    protected Money(int seats)
    {
        _seats = seats;
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