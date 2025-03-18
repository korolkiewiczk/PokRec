using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PT.Algorithm.Model;
using PT.Interfaces;

namespace PT.Algorithm
{
    public class WeightedMonteCarlo<T, TK> where T : IWeightedEncounter, IRandomGenerated<TK>, new()
    {
        private static readonly ConcurrentDictionary<TK, (int count, double better, double smaller)> PreviousResults = new();
        private readonly int _n;
        private readonly TK _arg;
        private readonly object _lockObject = new object();

        public WeightedMonteCarlo(int n, TK arg)
        {
            _n = n;
            _arg = arg;
        }

        public MonteCarloResult Solve()
        {
            double betterWeighted = 0;
            double smallerWeighted = 0;
            double totalWeight = 0;

            Parallel.For(0, _n, new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount}, 
                () => (betterLocal: 0.0, smallerLocal: 0.0, weightLocal: 0.0),
                (i, _, local) =>
                {
                    var item = new T();
                    item.Generate(_arg);
                    double weight = item.Weight;
                    
                    if (item.IsWinning) local.betterLocal += weight;
                    else if (item.IsLoosing) local.smallerLocal += weight;
                    
                    local.weightLocal += weight;
                    return local;
                },
                localSum =>
                {
                    lock (_lockObject)
                    {
                        betterWeighted += localSum.betterLocal;
                        smallerWeighted += localSum.smallerLocal;
                        totalWeight += localSum.weightLocal;
                    }
                });

            // Normalize by total weight instead of count
            double betterRatio = totalWeight > 0 ? betterWeighted / totalWeight : 0;
            double smallerRatio = totalWeight > 0 ? smallerWeighted / totalWeight : 0;
            
            var newResult = new MonteCarloResult(betterRatio, smallerRatio);

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