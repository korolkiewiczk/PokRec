using CfrSolver.Interfaces;
using CfrSolver.Model;

namespace CfrSolver.Cfr
{
    /// <summary>
    /// Implementation of the CFR+ algorithm.
    /// </summary>
    internal class CfrPlus : ICfr
    {
        private readonly int _player;
        private readonly int _hand;
        private readonly int _winningPlayer;

        public CfrPlus(int player, int hand, int winningPlayer)
        {
            _player = player;
            _hand = hand;
            _winningPlayer = winningPlayer;
        }

        /// <summary>
        /// Computes the counterfactual regret value for the given node.
        /// </summary>
        /// <param name="node">The current game tree node.</param>
        /// <param name="reachProb">The current reach probability (realization weight).</param>
        /// <returns>The computed expected value.</returns>
        public float Compute(Node node, float reachProb)
        {
            if (node.Round == Round.Fold)
            {
                return (node.Pos == _player ? -node.PayOff : node.PayOff) * reachProb;
            }

            if (node.Round == Round.Showdown)
            {
                if (_winningPlayer == -1) return 0;
                return (_player == _winningPlayer ? node.PayOff : -node.PayOff) * reachProb;
            }

            float[] strategy = node.GetStrategy(_hand, reachProb);
            float ev = 0;

            if (node.Pos == _player)
            {
                var utilities = new float[node.Children.Length];

                // First loop: compute utilities and accumulate expected value.
                for (int a = 0; a < node.Children.Length; a++)
                {
                    utilities[a] = Compute(node.Children[a], reachProb);
                    ev += strategy[a] * utilities[a];
                }

                // Second loop: update regrets.
                for (int a = 0; a < node.Children.Length; a++)
                {
                    node.UpdateCfr(_hand, a, utilities[a] - ev);
                }
            }
            else
            {
                // For opponent nodes, simply accumulate the EV from children.
                for (int a = 0; a < node.Children.Length; a++)
                {
                    float newReachProb = strategy[a] * reachProb;
                    float childEV = Compute(node.Children[a], newReachProb);
                    ev = (a == 0) ? childEV : ev + childEV;
                }
            }

            return ev;
        }
    }
}
