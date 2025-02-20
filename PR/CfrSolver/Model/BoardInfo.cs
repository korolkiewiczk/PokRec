namespace CfrSolver.Model
{
    public struct BoardInfo
    {
        public int Hand { get; set; }
        public int OppHand { get; set; }
        public int WinningPlayer { get; set; }

        public int[] Hands => [Hand, OppHand];
    }
}
