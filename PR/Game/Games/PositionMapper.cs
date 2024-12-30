using Game.MultiRegionMatchers;

namespace Game.Games
{
    public static class PositionMapper
    {
        // Define the “middle” positions, in the order you want them to appear
        // after seat #1 (Button) but before last two seats (BB, SB).
        // You can extend this list if you want more UTG+X seats for >10 handed, etc.
        private static readonly PokerPosition[] MiddlePositions = new[]
        {
            PokerPosition.CO,
            PokerPosition.HJ,
            PokerPosition.LJ,
            PokerPosition.UTG,
            PokerPosition.UTG1,
            PokerPosition.UTG2,
            PokerPosition.UTG3,
            PokerPosition.UTG4,
            PokerPosition.UTG5
            // etc. Add more if needed for very large tables.
        };

        public static PokerPosition GetPokerPosition(this Place place, int totalPlayers)
        {
            // If seat is not valid or no one is sitting:
            if (!place.HasPosition())
                return PokerPosition.None;

            // rawPos goes from 1..totalPlayers
            int rawPos = place.Pos;
            return MapToPokerPosition(rawPos, totalPlayers);
        }

        private static bool HasPosition(this Place place) => place.Count > 0;

        private static PokerPosition MapToPokerPosition(int rawPos, int totalPlayers)
        {
            // Special handling for very small games
            if (totalPlayers < 2)
                return PokerPosition.None;

            // 2-handed (Heads-Up) convention:
            if (totalPlayers == 2)
            {
                // seat 1 => Button (also SB in heads-up), seat 2 => BB
                return rawPos switch
                {
                    1 => PokerPosition.Button, // Also small blind
                    2 => PokerPosition.BigBlind,
                    _ => PokerPosition.None
                };
            }

            // 3-handed convention:
            if (totalPlayers == 3)
            {
                // seat 1 => Button, seat 2 => SB, seat 3 => BB
                return rawPos switch
                {
                    1 => PokerPosition.Button,
                    2 => PokerPosition.SmallBlind,
                    3 => PokerPosition.BigBlind,
                    _ => PokerPosition.None
                };
            }

            // For 4+ players, do:
            //  seat 1 => Button
            //  seat n => SmallBlind
            //  seat n-1 => BigBlind
            //  seats in between => CO, HJ, LJ, UTG, UTG1, UTG2, ...
            //
            // Example for 5-handed:
            //   seat 1 => BTN
            //   seat 2 => CO
            //   seat 3 => HJ  (or LJ, depending on how many seats we have)
            //   seat 4 => BB
            //   seat 5 => SB

            // If it’s seat #1 => Button
            if (rawPos == 1)
                return PokerPosition.Button;

            // If it’s seat #(totalPlayers) => SB
            if (rawPos == totalPlayers)
                return PokerPosition.SmallBlind;

            // If it’s seat #(totalPlayers - 1) => BB
            if (rawPos == totalPlayers - 1)
                return PokerPosition.BigBlind;

            // Now fill seats 2..(n-2) from MiddlePositions
            int middleCount = totalPlayers - 3; // how many seats are "in the middle"
            // seat #2 is the first of the “middle” seats, seat #(n-2) is the last
            // We want an index into MiddlePositions array that starts at 0 for seat #2
            int seatIndex = rawPos - 2; // zero-based index into MiddlePositions

            if (seatIndex >= 0 && seatIndex < MiddlePositions.Length && seatIndex < middleCount)
            {
                return MiddlePositions[seatIndex];
            }

            // If we somehow run out of positions or something else goes off:
            return PokerPosition.None;
        }
    }
}