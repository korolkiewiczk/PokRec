using System.Collections.Concurrent;
using System.Security.Cryptography;
using CfrSolver.Interfaces;
using CfrSolver.Model;

namespace CfrSolver;

public class TrainerParallel : ITrainer
{
    private readonly NodeGen _nodeGen;
    private readonly int _trainIterations;
    private readonly IHandGenerator _handGenerator;
    private readonly ICfrFactory _cfrFactory;

    // Dictionary to hold a lock object for each masked key.
    private readonly ConcurrentDictionary<int, object> _maskLocks;

    public TrainerParallel(NodeGen nodeGen, int trainIterations, IHandGenerator handGenerator, ICfrFactory cfrFactory)
    {
        _nodeGen = nodeGen;
        _trainIterations = trainIterations;
        _handGenerator = handGenerator;
        _cfrFactory = cfrFactory;
        _maskLocks = new ConcurrentDictionary<int, object>();
    }

    /// <summary>
    /// Runs training in parallel. For each training iteration, we compute a masked key based on the hand.
    /// If many iterations share the same masked key, they will be serialized; otherwise, they can update concurrently.
    /// </summary>
    /// <param name="eq">The accumulated equity from training.</param>
    /// <param name="possibleHands">A set of all hand identifiers encountered.</param>
    /// <param name="progress">An optional callback to report progress.</param>
    /// <returns>The root node of the trained game tree.</returns>
    public Node Train(out float eq, out HashSet<int> possibleHands, Action<int> progress = null)
    {
        // Generate the initial game tree.
        var rootNode = _nodeGen.Generate();

        var eqC = 0f;
        var concurrentHands = new ConcurrentBag<int>();

        // Global lock for safely updating the cumulative equity.
        object eqLock = new object();

        int iterations = 0;
        Parallel.For(0, _trainIterations, i =>
        {
            // Generate a random hand.
            HandInfo handInfo = _handGenerator.GenerateRandomHand();
            concurrentHands.Add(handInfo.Hand);

            float eq1 = 0, eq2 = 0;

            eq1 = _cfrFactory.Create(0, handInfo.Hand, handInfo.WinningPlayer).Compute(rootNode, 1);
            eq2 = _cfrFactory.Create(1, handInfo.Hand, 1 - handInfo.WinningPlayer).Compute(rootNode, 1);

            // Update the cumulative equity in a thread-safe manner.
            lock (eqLock)
            {
                eqC += eq1 + eq2;
            }

            // Optionally report progress.
            progress?.Invoke(iterations++);
        });

        possibleHands = [..concurrentHands];
        eq = eqC;
        return rootNode;
    }
}