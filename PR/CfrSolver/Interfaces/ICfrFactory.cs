namespace CfrSolver.Interfaces
{
    public interface ICfrFactory
    {
        ICfr Create(int player, int hand, int winningPlayer);
    }
}
