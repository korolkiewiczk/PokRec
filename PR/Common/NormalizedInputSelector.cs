using System.Collections.Generic;
using System.Linq;

namespace Common;

public static class NormalizedInputSelector
{
    public static string GetNormalizedFromInputs(List<string> inputs)
    {
        if (inputs == null || inputs.Count == 0)
            return string.Empty;

        var validNormalizedInputs = inputs.Select(input => input.ToUpper()).ToList();

        if (validNormalizedInputs.Count == 0)
            return string.Empty;

        var frequencyDict = validNormalizedInputs
            .GroupBy(n => n)
            .ToDictionary(g => g.Key, g => g.Count());

        return frequencyDict
            .OrderByDescending(kvp => kvp.Value)
            .First().Key;
    }
}