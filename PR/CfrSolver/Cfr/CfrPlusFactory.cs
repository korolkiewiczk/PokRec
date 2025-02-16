using CfrSolver.Interfaces;

namespace CfrSolver.Cfr
{
    public class CfrPlusFactory : ICfrFactory
    {
        public ICfr Create(int player, int hand, int winningPlayer)
        {
            return new CfrPlus(player, hand, winningPlayer);
            //return new MonteCarloCfr(player, hand, winningPlayer);
        }
    }
}
