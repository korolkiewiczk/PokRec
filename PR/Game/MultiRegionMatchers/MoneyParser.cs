using System;
using System.Linq;

namespace Game.MultiRegionMatchers;

public static class MoneyParser
{
    public static decimal? ParseMoneyValue(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return null;
        }

        string cleanText = new string(rawText.Where(char.IsDigit).ToArray());
        var parsed = decimal.TryParse(cleanText, out decimal value);
        return parsed ? value : rawText.Contains("all", StringComparison.OrdinalIgnoreCase) ||
                                rawText.Contains("in", StringComparison.OrdinalIgnoreCase)? 0 : null;
    }
}