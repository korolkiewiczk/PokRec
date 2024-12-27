using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PT.Algorithm.Model;
using PT.Interfaces;

namespace PT.Algorithm
{
    public class MonteCarlo<T, TK> where T : IEncounter, IRandomGenerated<TK>, new()
    {
        private static readonly ConcurrentDictionary<TK, (int count, double better, double smaller)> PreviousResults = new();
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
            var newResult = new MonteCarloResult(better * reciprocal, smaller * reciprocal);

            PreviousResults.AddOrUpdate(_arg, 
                (1, newResult.Better, newResult.Smaller), 
                (key, oldValue) => 
                {
                    var newCount = oldValue.count + 1;
                    var updatedBetter = (oldValue.better * oldValue.count + newResult.Better) / newCount;
                    var updatedSmaller = (oldValue.smaller * oldValue.count + newResult.Smaller) / newCount;
                    return (newCount, updatedBetter, updatedSmaller);
                });

            var cumulativeResult = PreviousResults[_arg];
            return new MonteCarloResult(cumulativeResult.better, cumulativeResult.smaller);
        }
    }
}