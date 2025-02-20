using CfrSolver.Model;

namespace CfrSolver.Utils;

public static class NodeExtensions
{
    /// <summary>
    /// Gets the average strategy for the specified hand.
    /// </summary>
    public static float[] GetAverageStrategy(this Node node, int hand)
    {
        var maskedHand = hand & node.ComputeMask();
        bool found = node.Data.TryGetValue(maskedHand, out var data);
        if (!found) return [];
        float normalizingSum = 0;
        float[] avgStrategy = new float[node.Children.Length];
        for (int a = 0; a < node.Children.Length; a++)
        {
            normalizingSum += data.StrategySum[a];
        }

        for (int a = 0; a < node.Children.Length; a++)
        {
            avgStrategy[a] = normalizingSum > 0
                ? data.StrategySum[a] / normalizingSum
                : 1.0f / node.Children.Length;
        }

        return avgStrategy;
    }
}