namespace CfrSolver.Model
{
    public record BoardInfo
    {
        public int Hand { get; set; }
        public int OppHand { get; set; }
        public int WinningPlayer { get; set; }
    }
}
