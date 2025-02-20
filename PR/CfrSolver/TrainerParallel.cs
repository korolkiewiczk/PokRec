using System.Collections.Concurrent;
using System.Security.Cryptography;
using CfrSolver.Interfaces;
using CfrSolver.Model;

namespace CfrSolver;

public class TrainerParallel : ITrainer
{
    private readonly NodeGen _nodeGen;
    private readonly int _trainIterations;
    private readonly IBoardGenerator _boardGenerator;
    private readonly ICfrFactory _cfrFactory;

    public TrainerParallel(NodeGen nodeGen, int trainIterations, IBoardGenerator boardGenerator, ICfrFactory cfrFactory)
    {
        _nodeGen = nodeGen;
        _trainIterations = trainIterations;
        _boardGenerator = boardGenerator;
        _cfrFactory = cfrFactory;
    }

    /// <summary>
    /// Runs training in parallel.
    /// </summary>
    /// <param name="eq">The accumulated equity from training.</param>
    /// <param name="possibleHands">A set of all hand identifiers encountered.</param>
    /// <param name="progress">An optional callback to report progress.</param>
    /// <param name="cancellationToken">Token to support cancellation of the operation.</param>
    /// <returns>The root node of the trained game tree.</returns>
    public Node Train(out float eq, out HashSet<int> possibleHands, Action<int> progress = null, 
        CancellationToken cancellationToken = default)
    {
        // Generate the initial game tree.
        var rootNode = _nodeGen.Generate();

        var eqC = 0f;
        var concurrentHands = new ConcurrentBag<int>();

        // Global lock for safely updating the cumulative equity.
        object eqLock = new object();

        int iterations = 0;
        var parallelOptions = new ParallelOptions 
        { 
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount 
        };

        Parallel.For(0, _trainIterations, parallelOptions, i =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Generate a random hand.
            BoardInfo boardInfo = _boardGenerator.GenerateBoardAbstraction();
            concurrentHands.Add(boardInfo.Hand);

            float eq1 = 0, eq2 = 0;

            eq1 = _cfrFactory.Create(0, boardInfo.Hand, boardInfo.WinningPlayer).Compute(rootNode, 1);
            eq2 = _cfrFactory.Create(1, boardInfo.Hand, boardInfo.WinningPlayer == -1 ? -1 : 1 - boardInfo.WinningPlayer).Compute(rootNode, 1);

            // Update the cumulative equity in a thread-safe manner.
            lock (eqLock)
            {
                eqC += eq1 + eq2;
            }

            // Optionally report progress.
            progress?.Invoke(iterations++);
        });

        possibleHands = new HashSet<int>(concurrentHands);
        eq = eqC;
        return rootNode;
    }
}