namespace CfrSolver.Model
{
    public struct Action
    {
        public OpType OpType { get; }
        public short Bet { get; }

        public static Action Initial(int b) => new Action(OpType.Raise, (short)b);
        public static Action Invalid => new Action(OpType.Fold, -1);

        public Action(OpType opType)
        {
            if (opType == OpType.Raise)
            {
                throw new InvalidOperationException("Call 2 arg ctor for Raise operation.");
            }

            OpType = opType;
            Bet = 0;
        }

        public Action(OpType opType, int bet)
        {
            OpType = opType;
            Bet = (short)bet;
        }

        public override string ToString()
        {
            return $"[{OpType} {Bet}]";
        }

        public string ToShortString()
        {
            return $"{OpType.ToString()[0]}{(Bet != 0 ? Bet.ToString() : "")}";
        }
    }
}