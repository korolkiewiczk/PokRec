using System;
using System.Collections.Generic;

namespace Common;

public static class FuzzyKeyFinder
{
    public static string FindSimilarKey(IEnumerable<string> keys, string target, int maxDistance)
    {
        string bestMatch = null;
        var bestDistance = int.MaxValue;

        foreach (var key in keys)
        {
            var distance = ComputeLevenshteinDistance(key, target);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestMatch = key;
            }
        }

        return bestDistance <= maxDistance ? bestMatch : null;
    }

    private static int ComputeLevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
            return string.IsNullOrEmpty(t) ? 0 : t.Length;
        if (string.IsNullOrEmpty(t))
            return s.Length;

        var d = new int[s.Length + 1, t.Length + 1];

        for (var i = 0; i <= s.Length; i++)
            d[i, 0] = i;
        for (var j = 0; j <= t.Length; j++)
            d[0, j] = j;

        for (var i = 1; i <= s.Length; i++)
        {
            for (var j = 1; j <= t.Length; j++)
            {
                var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[s.Length, t.Length];
    }
}