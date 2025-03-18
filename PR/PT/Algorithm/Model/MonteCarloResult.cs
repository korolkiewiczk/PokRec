namespace PT.Algorithm.Model
{
    public readonly record struct MonteCarloResult(double Better, double Smaller)
    {
        public double Exact => (1 - Better - Smaller);
    }
}
