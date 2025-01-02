namespace emu.lib;

public record WeightedImages(string ImagePath, double Score) : IComparable<WeightedImages>
{
    public int CompareTo(WeightedImages other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return Score.CompareTo(other.Score);
    }
}