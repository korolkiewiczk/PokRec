namespace Game.Games
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
        LJ = 7,          // Lojack
        HJ = 8,          // Hijack
        CO = 9,          // Cutoff
    }
} 