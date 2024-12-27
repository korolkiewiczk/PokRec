namespace PT.Algorithm.Model
{
    public readonly struct MonteCarloResult
    {
        public double Better { get; }

        public double Exact => (1 - Better - Smaller);

        public double Smaller { get; }

        public MonteCarloResult(double better, double smaller)
        {
            Better = better;
            Smaller = smaller;
        }
    }
}
