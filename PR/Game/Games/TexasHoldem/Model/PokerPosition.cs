namespace Game.Games.TexasHoldem.Model
{
    public enum PokerPosition
    {
        None = 0,
        Button = 1,      // Dealer
        SmallBlind = 2,
        BigBlind = 3,
        UTG = 4,         // Under The Gun
        UTG1 = 5,        // UTG+1
        UTG2 = 6,        // UTG+2
        UTG3 = 7,        // UTG+2
        UTG4 = 8,        // UTG+2
        UTG5 = 9,        // UTG+2
        LJ = 10,          // Lojack
        HJ = 11,          // Hijack
        CO = 12,          // Cutoff
    }
} 