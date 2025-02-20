using CfrSolver.Model;

namespace CfrSolver;

public interface ITrainer
{
    Node Train(out float eq, out HashSet<int> possibleHands, Action<int> progress = null, 
        System.Threading.CancellationToken cancellationToken = default);
}