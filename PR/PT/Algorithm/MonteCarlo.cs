using System.Threading;
using System.Threading.Tasks;
using PT.Algorithm.Model;
using PT.Interfaces;

namespace PT.Algorithm
{
    public class MonteCarlo<T, TK> where T : IEncounter, IRandomGenerated<TK>, new()
    {
        private readonly int _n;
        private readonly TK _arg;

        public MonteCarlo(int n, TK arg)
        {
            _n = n;
            _arg = arg;
        }

        public MonteCarloResult Solve()
        {
            var better = 0;
            var smaller = 0;

            Parallel.For(0, _n, () => (betterLocal: 0, smallerLocal: 0),
                (i, _, local) =>
                {
                    var item = new T();
                    item.Generate(_arg);
                    if (item.IsWinning) local.betterLocal++;
                    else if (item.IsLoosing) local.smallerLocal++;
                    return local;
                },
                localSum =>
                {
                    Interlocked.Add(ref better, localSum.betterLocal);
                    Interlocked.Add(ref smaller, localSum.smallerLocal);
                });

            var reciprocal = 1.0 / _n;
            return new MonteCarloResult(better * reciprocal, smaller * reciprocal);
        }
    }
}