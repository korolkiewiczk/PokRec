using CfrSolver.Interfaces;
using CfrSolver.Model;

namespace CfrSolver.Cfr
{
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

        public float Compute(Node node, float op)
        {
            if (node.Round == Round.Fold)
            {
                return (node.Pos == _player ? -node.PayOff : node.PayOff) * op;
            }

            if (node.Round == Round.Showdown)
            {
                if (_winningPlayer == -1) return 0;
                return (_player == _winningPlayer ? node.PayOff : -node.PayOff) * op;
            }

            float[] s = node.GetStrategy(_hand, op);

            float ev = 0;

            if (node.Pos == _player)
            {
                var u = new float[node.Children.Length];

                for (int a = 0; a < node.Children.Length; a++)
                {
                    u[a] = Compute(node.Children[a], op);

                    ev += s[a] * u[a];
                }

                for (var a = 0; a < node.Children.Length; a++)
                {
                    node.UpdateCfr(_hand, a, u[a] - ev);
                }
            }
            else
            {
                for (var a = 0; a < node.Children.Length; a++)
                {
                    float newop = s[a] * op;

                    CopyOrAdd(a == 0, ref ev, Compute(node.Children[a], newop));
                }
            }

            return ev;
        }

        private void CopyOrAdd(bool isNew, ref float ev, float vanillaCfrPlus)
        {
            if (isNew)
            {
                ev = vanillaCfrPlus;
            }
            else
            {
                ev += vanillaCfrPlus;
            }
        }
    }
}
