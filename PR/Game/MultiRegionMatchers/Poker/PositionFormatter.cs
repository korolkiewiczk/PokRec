namespace Game.MultiRegionMatchers.Poker
{
    public static class PositionFormatter
    {
        public static string ToDisplayString(this PokerPosition position)
        {
            return position switch
            {
                PokerPosition.None => "",
                PokerPosition.Button => "BTN",
                PokerPosition.SmallBlind => "SB",
                PokerPosition.BigBlind => "BB",
                PokerPosition.UTG => "UTG",
                PokerPosition.UTG1 => "UTG+1",
                PokerPosition.UTG2 => "UTG+2",
                PokerPosition.LJ => "LJ",
                PokerPosition.HJ => "HJ",
                PokerPosition.CO => "CO",
                _ => "?"
            };
        }
    }
} 