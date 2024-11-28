using Game.MultiRegionMatchers;

namespace Game.Games
{
    public static class PositionMapper
    {
        public static PokerPosition GetPokerPosition(this Place place, int totalPlayers)
        {
            if (!place.HasPosition()) 
                return PokerPosition.None;
            
            int rawPos = place.Pos;
            return MapToPokerPosition(rawPos, totalPlayers);
        }

        private static bool HasPosition(this Place place) => place.Count > 0;

        private static PokerPosition MapToPokerPosition(int rawPos, int totalPlayers)
        {
            // Basic positions that exist in all games
            return rawPos switch
            {
                1 => PokerPosition.Button,
                2 when totalPlayers == 2 => PokerPosition.BigBlind,
                2 => PokerPosition.SmallBlind,
                3 => PokerPosition.BigBlind,
                _ when rawPos == totalPlayers => PokerPosition.CO,
                _ when totalPlayers == 2 => PokerPosition.None,
                _ => MapRemainingPositions(rawPos, totalPlayers)
            };
        }

        private static PokerPosition MapRemainingPositions(int rawPos, int totalPlayers)
        {
            // Handle positions after BB (rawPos > 3)
            if (totalPlayers <= 3) return PokerPosition.None;
            
            // Map positions based on distance from button
            return (totalPlayers, rawPos) switch
            {
                // 4 players: UTG is CO
                (4, 4) => PokerPosition.CO,
                
                // 5 players: UTG is HJ, UTG+1 is CO
                (5, 4) => PokerPosition.HJ,
                (5, 5) => PokerPosition.CO,
                
                // 6+ players: Standard mapping
                (_, 4) => PokerPosition.UTG,
                (_, 5) when totalPlayers >= 8 => PokerPosition.UTG1,
                (_, 5) => PokerPosition.LJ,
                (_, 6) when totalPlayers >= 9 => PokerPosition.UTG2,
                (_, 6) => PokerPosition.LJ,
                (_, 7) => PokerPosition.LJ,
                (_, 8) => PokerPosition.HJ,
                (_, >= 9) => PokerPosition.CO,
                
                _ => PokerPosition.None
            };
        }
    }
} 