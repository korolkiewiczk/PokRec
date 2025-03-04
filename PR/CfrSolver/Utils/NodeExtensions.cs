using CfrSolver.Model;

namespace CfrSolver.Utils;

public static class NodeExtensions
{
    /// <summary>
    /// Gets the average strategy for the specified hand.
    /// </summary>
    public static float[] GetAverageStrategy(this Node node, int hand)
    {
        int maskedHand = hand & node.ComputeMask();
        if (!node.Data.TryGetValue(maskedHand, out var data))
        {
            // Return a default uniform strategy if no data exists.
            float[] uniform = new float[node.Children.Length];
            for (int a = 0; a < uniform.Length; a++)
                uniform[a] = 1.0f / node.Children.Length;
            return uniform;
        }

        float normalizingSum = data.StrategySum.Sum();
        float[] avgStrategy = new float[node.Children.Length];
        for (int a = 0; a < node.Children.Length; a++)
        {
            avgStrategy[a] = normalizingSum > 0 ? data.StrategySum[a] / normalizingSum : 1.0f / node.Children.Length;
        }

        return avgStrategy;
    }

    /// <summary>
    /// Recursively traverses the CFR tree according to the provided action sequence.
    /// </summary>
    public static Node GetNodeForActionSequence(this Node currentNode, string[] actions)
    {
        if (actions.Length == 0)
        {
            return currentNode;
        }

        foreach (var child in currentNode.Children)
        {
            if (child.Action.ToShortString().Equals(actions[0], StringComparison.OrdinalIgnoreCase))
            {
                string[] remaining = actions.Skip(1).ToArray();
                var result = GetNodeForActionSequence(child, remaining);
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// The result of an interpolated lookup in the CFR tree.
    /// Contains the matching node and a penalty (≥1) representing the distance.
    /// </summary>
    public record InterpolatedNodeResult(Node Node, float Penalty);

    /// <summary>
    /// Recursively traverses the CFR tree according to the provided action sequence.
    /// If an exact match is not found at any level, it picks the child that minimizes a distance metric.
    /// The overall penalty is the product of per-level penalties.
    /// </summary>
    public static InterpolatedNodeResult GetInterpolatedNodeForActionSequence(this Node currentNode, string[] actions)
    {
        if (actions.Length == 0)
        {
            return new InterpolatedNodeResult(currentNode, 1.0f);
        }

        string currentAction = actions[0];

        // Try to find an exact match first.
        foreach (var child in currentNode.Children)
        {
            if (child.Action.ToShortString().Equals(currentAction, StringComparison.OrdinalIgnoreCase))
            {
                return GetInterpolatedNodeForActionSequence(child, actions.Skip(1).ToArray());
            }
        }

        // No exact match: find the child with minimal distance.
        float bestDistance = MaxPenalty;
        Node bestChild = null;
        foreach (var child in currentNode.Children)
        {
            float distance = GetActionDistance(child.Action.ToShortString(), currentAction);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestChild = child;
            }
        }

        if (bestChild == null)
        {
            // Fallback (should not occur in a valid tree).
            return new InterpolatedNodeResult(currentNode, MaxPenalty);
        }

        var childResult = GetInterpolatedNodeForActionSequence(bestChild, actions.Skip(1).ToArray());
        // Combine the penalty for this level (1 + bestDistance) with deeper penalty.
        float combinedPenalty = (1.0f + bestDistance) * childResult.Penalty;
        return childResult with {Penalty = combinedPenalty};
    }

    private const float MaxPenalty = 10.0f;

    /// <summary>
    /// Computes a distance between two action strings.
    /// For raise/all-in actions (starting with 'R' or 'A'), compares numeric values;
    /// if the action types differ, a high penalty is returned.
    /// </summary>
    private static float GetActionDistance(string actual, string target)
    {
        if (actual.Equals(target, StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        char typeActual = actual[0];
        char typeTarget = target[0];
        if (typeActual != typeTarget)
        {
            return MaxPenalty; // Very different actions incur a high penalty.
        }

        if (typeActual == nameof(OpType.Raise)[0] || typeActual == nameof(OpType.All)[0])
        {
            if (actual.Length > 1 && target.Length > 1 &&
                int.TryParse(actual.Substring(1), out int numberActual) &&
                int.TryParse(target.Substring(1), out int numberTarget))
            {
                return CalculatePenalty(numberActual, numberTarget, MaxPenalty);
            }

            return MaxPenalty;
        }

        // For actions like "F" or "C", if not an exact match, assign a high penalty.
        return MaxPenalty;
    }
    
    private static float CalculatePenalty(float numberActual, float numberTarget, float maxPenalty)
    {
        if (numberActual == 0) return 1;
        // Ograniczamy wynik do MaxPenalty, jeśli numberActual jest poza zakresem [0.5 * numberTarget, 2 * numberTarget]
        if (numberActual < 0.5 * numberTarget || numberActual > 2 * numberTarget)
        {
            return maxPenalty;
        }
        var ratio = Math.Abs(numberActual - numberTarget) / numberTarget;
        var penalty = ratio * maxPenalty;
        return penalty;
    }
}