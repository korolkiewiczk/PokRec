namespace PT.Algorithm.Model
{
    public struct MonteCarloResult
    {
        private readonly double _better;
        private readonly double _smaller;

        public double Better
        {
            get { return _better; }
        }

        public double Exact
        {
            get { return (1 - Better - Smaller); }
        }

        public double Smaller
        {
            get { return _smaller; }
        }

        public MonteCarloResult(double better, double smaller)
        {
            _better = better;
            _smaller = smaller;
        }
    }
}
