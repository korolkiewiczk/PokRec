namespace CfrSolver.Model
{
    public struct Node
    {
        private readonly Dictionary<int, float[]> _cfr;
        private readonly Dictionary<int, float[]> _strategy;
        private readonly Dictionary<int, float[]> _strategySum;

        public Node(byte pos, Action action, Round round, Node[] children, int payOff = 0)
        {
            Pos = pos;
            Action = action;
            Round = round;
            Children = children;
            PayOff = (short)payOff;

            _cfr = new Dictionary<int, float[]>();
            _strategy = new Dictionary<int, float[]>();
            _strategySum = new Dictionary<int, float[]>();
        }

        public byte Pos { get; }

        public Action Action { get; }

        public Round Round { get; }

        public Node[] Children { get; }

        public short PayOff { get; }

        public static bool IsTerminal(Round round) => round == Round.Fold || round == Round.Showdown;
        public bool IsTerminal() => IsTerminal(Round);

        public override string ToString()
        {
            return $"P={Pos} A={Action} S={Round}" + (IsTerminal() ? $" PAY={PayOff}" : "");
        }

        public string ToStringFull(int hand)
        {
            string part1 = ToString();
            string part2 = string.Join(";", GetAverageStrategy(hand));
            return $"{part1} [{part2}]";
        }

        public float[] GetStrategy(int hand, float realizationWeight)
        {
            float normalizingSum = 0;
            float[] strategy = Strategy(hand);
            float[] cfr = Cfr(hand);
            float[] strategySum = StrategySum(hand);

            for (int a = 0; a < Children.Length; a++)
            {
                strategy[a] = cfr[a] > 0 ? cfr[a] : 0;
                normalizingSum += strategy[a];
            }

            for (int a = 0; a < Children.Length; a++)
            {
                if (normalizingSum > 0)
                {
                    strategy[a] /= normalizingSum;
                }
                else
                {
                    strategy[a] = 1.0f / Children.Length;
                }

                strategySum[a] += realizationWeight * strategy[a];
            }

            return strategy;
        }

        public float[] GetAverageStrategy(int hand)
        {
            float[] strategySum = StrategySum(hand);
            float[] avgStrategy = new float[Children.Length];
            float normalizingSum = 0;

            for (int a = 0; a < Children.Length; a++)
            {
                normalizingSum += strategySum[a];
            }

            for (int a = 0; a < Children.Length; a++)
            {
                if (normalizingSum > 0)
                {
                    avgStrategy[a] = strategySum[a] / normalizingSum;
                }
                else
                {
                    avgStrategy[a] = 1.0f / Children.Length;
                }
            }
            return avgStrategy;
        }

        public void UpdateCfr(int hand, int i, float delta)
        {
            hand = GetHand(hand);
            _cfr[hand][i] += delta;
            //plus
            _cfr[hand][i] = Math.Max(0, _cfr[hand][i]);
            _strategy[hand][i] = _cfr[hand][i];
        }

        private float[] Cfr(int hand)
        {
            hand = GetHand(hand);

            if (!_cfr.ContainsKey(hand))
            {
                _cfr[hand] = new float[Children.Length];
            }

            return _cfr[hand];
        }

        private float[] Strategy(int hand)
        {
            hand = GetHand(hand);

            if (!_strategy.ContainsKey(hand))
            {
                _strategy[hand] = new float[Children.Length];
            }

            return _strategy[hand];
        }

        private float[] StrategySum(int hand)
        {
            hand = GetHand(hand);

            if (!_strategySum.ContainsKey(hand))
            {
                _strategySum[hand] = new float[Children.Length];
            }

            return _strategySum[hand];
        }

        private int GetHand(int hand)
        {
            return hand & ((16 << (4 * (int)Round)) - 1);
        }
    }
}
