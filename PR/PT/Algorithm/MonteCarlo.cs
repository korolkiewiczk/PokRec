using PT.Algorithm.Model;
using PT.Interfaces;

namespace PT.Algorithm
{
    public class MonteCarlo<T, TK> where T : IEncounter, IRandomGenerated<TK>, new()
    {
        private readonly int _n;
        private readonly T _item;
        private readonly TK _arg;

        public MonteCarlo(T item, int n, TK arg)
        {
            _item = item;
            _n = n;
            _arg = arg;
        }

        public MonteCarloResult Solve()
        {
            int better = 0;
            int smaller = 0;
            T item = new T();
            for (int i = 0; i < _n; i++)
            {
                item.Generate(_arg);
                bool isWinning = item.IsWinning;
                bool isLoosing = item.IsLoosing;
                if (isWinning)
                {
                    ++better;
                }
                else 
                if (isLoosing)
                {
                    ++smaller;
                }
            }
            return new MonteCarloResult((double)better / _n, (double)smaller / _n);
        }
    }
}
