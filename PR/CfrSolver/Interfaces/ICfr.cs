using CfrSolver.Model;

namespace CfrSolver.Interfaces
{
    public interface ICfr
    {
        float Compute(Node node, float op);
    }
}