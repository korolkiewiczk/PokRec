using CfrSolver.Interfaces;
using CfrSolver.Model;

namespace CfrSolver
{
    public class Trainer
    {
        private readonly NodeGen _nodeGen;
        private readonly int _trainIterations;
        private readonly IHandGenerator _handGenerator;
        private readonly ICfrFactory _cfrFactory;

        public Trainer(NodeGen nodeGen, int trainIterations, IHandGenerator handGenerator, ICfrFactory cfrFactory)
        {
            _nodeGen = nodeGen;
            _trainIterations = trainIterations;

            _handGenerator = handGenerator;
            _cfrFactory = cfrFactory;
        }

        public Node Train(out float eq, out HashSet<int> possibleHands, Action<int> progress = null)
        {
            var rootNode = _nodeGen.Generate();

            eq = 0;

            possibleHands = new HashSet<int>();

            for (int i = 0; i < _trainIterations; i++)
            {
                HandInfo handInfo = _handGenerator.GenerateRandomHand();

                possibleHands.Add(handInfo.Hand);

                var eq1 = _cfrFactory.Create(0, handInfo.Hand, handInfo.WinningPlayer).Compute(rootNode, 1);
                var eq2 = _cfrFactory.Create(1, handInfo.Hand, 1 - handInfo.WinningPlayer).Compute(rootNode, 1);
                eq += eq1 + eq2;

                progress?.Invoke(i);
            }

            return rootNode;
        }
    }
}
