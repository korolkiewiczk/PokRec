using System.Collections.Generic;
using System.Linq;

namespace Game.Utils;

public static class ProbabilityUtils
{
    public static void NormalizeProbabilities(Dictionary<string, float> actionProbabilities, float? totalProbabilities = null)
    {
        var totalProbability = totalProbabilities ?? actionProbabilities.Values.Sum();
        if (totalProbability > 0)
        {
            foreach (var key in actionProbabilities.Keys.ToList())
            {
                actionProbabilities[key] /= totalProbability;
            }
        }
    }
}