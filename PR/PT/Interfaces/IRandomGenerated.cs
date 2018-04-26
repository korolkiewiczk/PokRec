namespace PT.Interfaces
{
    public interface IRandomGenerated<in T>
    {
        void Generate(T arg);
    }
}
