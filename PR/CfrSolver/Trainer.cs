using CfrSolver.Interfaces;
using CfrSolver.Model;

namespace CfrSolver
{
    public class Trainer : ITrainer
    {
        private readonly NodeGen _nodeGen;
        private readonly int _trainIterations;
        private readonly IBoardGenerator _boardGenerator;
        private readonly ICfrFactory _cfrFactory;

        public Trainer(NodeGen nodeGen, int trainIterations, IBoardGenerator boardGenerator, ICfrFactory cfrFactory)
        {
            _nodeGen = nodeGen;
            _trainIterations = trainIterations;

            _boardGenerator = boardGenerator;
            _cfrFactory = cfrFactory;
        }

        public Node Train(out float eq, out HashSet<int> possibleHands, Action<int> progress = null, CancellationToken cancellationToken = default)
        {
            var rootNode = _nodeGen.Generate();

            eq = 0;

            possibleHands = new HashSet<int>();

            for (int i = 0; i < _trainIterations; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return rootNode;
                }
                BoardInfo boardInfo = _boardGenerator.GenerateBoardAbstraction();

                possibleHands.Add(boardInfo.Hand);

                var eq1 = _cfrFactory.Create(0, boardInfo.Hand, boardInfo.WinningPlayer).Compute(rootNode, 1);
                var eq2 = _cfrFactory.Create(1, boardInfo.Hand, boardInfo.WinningPlayer == -1 ? -1 :1 - boardInfo.WinningPlayer).Compute(rootNode, 1);
                eq += eq1 + eq2;

                progress?.Invoke(i);
            }

            return rootNode;
        }
    }
}
