using CfrSolver.Interfaces;
using CfrSolver.Model;

namespace CfrSolver.Cfr;

/// <summary>
/// Implements a Monte Carlo Counterfactual Regret Minimization (MCCFR) algorithm using external sampling.
/// For the player’s own nodes, all actions are evaluated to update regrets.
/// For opponent nodes, a single action is sampled.
/// </summary>
internal class MonteCarloCfr : ICfr
{
    private readonly int _player;
    private readonly int _hand;
    private readonly int _winningPlayer;
    private readonly Random _rng;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonteCarloCfr"/> class.
    /// </summary>
    /// <param name="player">Index of the player for whom the regrets are updated.</param>
    /// <param name="hand">An identifier for the current hand.</param>
    /// <param name="winningPlayer">The index of the winning player at showdown (-1 if undefined).</param>
    /// <param name="seed">Optional seed for the random number generator.</param>
    public MonteCarloCfr(int player, int hand, int winningPlayer, int? seed = null)
    {
        _player = player;
        _hand = hand;
        _winningPlayer = winningPlayer;
        _rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>
    /// Computes the counterfactual value for the given node using external sampling.
    /// </summary>
    /// <param name="node">The current game tree node.</param>
    /// <param name="reachProb">The current reach probability (realization weight).</param>
    /// <returns>The computed expected value.</returns>
    public float Compute(Node node, float reachProb)
    {
        // Terminal nodes: fold or showdown.
        if (node.Round == Round.Fold)
        {
            return (node.Pos == _player ? -node.PayOff : node.PayOff) * reachProb;
        }

        if (node.Round == Round.Showdown)
        {
            if (_winningPlayer == -1) return 0;
            return (_player == _winningPlayer ? node.PayOff : -node.PayOff) * reachProb;
        }

        // Retrieve the current strategy for this node.
        float[] strategy = node.GetStrategy(_hand, reachProb);

        // If the node belongs to our player, evaluate all actions.
        if (node.Pos == _player)
        {
            float[] utilities = new float[node.Children.Length];
            float ev = 0;

            // Evaluate all children and compute the expected value.
            for (int a = 0; a < node.Children.Length; a++)
            {
                utilities[a] = Compute(node.Children[a], reachProb);
                ev += strategy[a] * utilities[a];
            }

            // Update regrets for each action.
            for (int a = 0; a < node.Children.Length; a++)
            {
                node.UpdateCfr(_hand, a, utilities[a] - ev);
            }

            return ev;
        }
        else
        {
            // For opponent nodes, sample one action based on the strategy.
            int sampledAction = SampleAction(strategy);
            float newReachProb = strategy[sampledAction] * reachProb;
            return Compute(node.Children[sampledAction], newReachProb);
        }
    }

    /// <summary>
    /// Samples an action index from the strategy distribution.
    /// </summary>
    /// <param name="strategy">An array of probabilities for each action.</param>
    /// <returns>The index of the sampled action.</returns>
    private int SampleAction(float[] strategy)
    {
        double r = _rng.NextDouble();
        double cumulative = 0;
        for (int a = 0; a < strategy.Length; a++)
        {
            cumulative += strategy[a];
            if (r < cumulative)
            {
                return a;
            }
        }
        // In case of floating point imprecision, return the last action.
        return strategy.Length - 1;
    }
}