namespace PT.Interfaces
{
    public interface IWeightedEncounter : IEncounter
    {
        double Weight { get; set; }
    }
} 