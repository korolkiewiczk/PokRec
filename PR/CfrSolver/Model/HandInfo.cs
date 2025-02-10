namespace CfrSolver.Model
{
    public struct HandInfo
    {
        public int Hand { get; set; }
        public int OppHand { get; set; }
        public int WinningPlayer { get; set; }

        public int[] Hands => new[] { Hand, OppHand };
    }
}
