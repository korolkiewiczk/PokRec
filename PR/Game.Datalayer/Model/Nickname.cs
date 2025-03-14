namespace Game.Datalayer.Model;

public class Nickname
{
    public int NicknameId { get; set; }
    public string Name { get; set; } = null!;
    public int Frequency { get; set; }

    public Stats Stats { get; set; } = null!;
}