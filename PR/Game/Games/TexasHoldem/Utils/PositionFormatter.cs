using Game.Games.TexasHoldem.Model;

namespace Game.Games.TexasHoldem.Utils
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
                PokerPosition.UTG3 => "UTG+3",
                PokerPosition.UTG4 => "UTG+4",
                PokerPosition.UTG5 => "UTG+5",
                PokerPosition.LJ => "LJ",
                PokerPosition.HJ => "HJ",
                PokerPosition.CO => "CO",
                _ => "?"
            };
        }
    }
} 