namespace CfrSolver.Model
{
    public record struct PlayerAction
    {
        public OpType OpType { get; set; }
        public int Bet { get; set; }

        public static PlayerAction Initial(int b) => b == 0 ? new PlayerAction(OpType.Call) : new PlayerAction(OpType.Raise, (short) b);
        public static PlayerAction Invalid => new PlayerAction(OpType.Fold, -1);

        public PlayerAction(OpType opType)
        {
            if (opType == OpType.Raise)
            {
                throw new InvalidOperationException("Call 2 arg ctor for Raise operation.");
            }

            OpType = opType;
            Bet = 0;
        }

        public PlayerAction(OpType opType, int bet)
        {
            OpType = opType;
            Bet = bet;
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