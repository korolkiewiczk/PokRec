using Game.Datalayer.Model;
using Microsoft.EntityFrameworkCore;
using Game.Common.Model;
using Common;
using Game.Common.Utils;

namespace Game.Datalayer
{
    public class GameRepository
    {
        private readonly GameDbContext _context;

        public GameRepository()
        {
            _context = new GameDbContext();
            _context.Database.EnsureCreated();
        }

        public void AddStats(Stats stats)
        {
            _context.Stats.Add(stats);
            _context.SaveChanges();
        }

        public Stats? GetStatsById(int id)
        {
            return _context.Stats.Include(s => s.Nickname).FirstOrDefault(s => s.NicknameId == id);
        }

        public List<Stats> GetAllStats()
        {
            return _context.Stats.Include(s => s.Nickname).ToList();
        }

        public void AddNickname(Nickname nickname)
        {
            _context.Nicknames.Add(nickname);
            _context.SaveChanges();
        }

        public Nickname GetNicknameById(int nicknameId)
        {
            return _context.Nicknames.FirstOrDefault(n => n.NicknameId == nicknameId);
        }

        public List<Nickname> GetAllNicknames()
        {
            return _context.Nicknames.ToList();
        }

        public void UpdateStats(Stats stats)
        {
            _context.Stats.Update(stats);
            _context.SaveChanges();
        }

        public void UpdateNickname(Nickname nickname)
        {
            _context.Nicknames.Update(nickname);
            _context.SaveChanges();
        }

        public void DeleteStats(int id)
        {
            var stats = _context.Stats.Find(id);
            if (stats != null)
            {
                _context.Stats.Remove(stats);
                _context.SaveChanges();
            }
        }

        public void DeleteNickname(int id)
        {
            var nickname = _context.Nicknames.Find(id);
            if (nickname != null)
            {
                _context.Nicknames.Remove(nickname);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Finds a similar nickname in the database using fuzzy matching
        /// </summary>
        private Nickname FindSimilarNicknameInDb(List<Nickname> dbNicknames, string playerName)
        {
            // Get all player names from the database
            var playerNames = dbNicknames.Select(n => n.Name).ToList();
            
            // Find similar key using fuzzy matching
            var similarKey = FuzzyKeyFinder.FindSimilarKey(playerNames, playerName, maxDistance: 2);
            
            if (similarKey != null)
            {
                // Find the nickname with the highest frequency among those with the similar key
                return dbNicknames
                    .Where(n => n.Name == similarKey)
                    .FirstOrDefault();
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the next available NicknameId
        /// </summary>
        private int GetNextNicknameId()
        {
            return _context.Nicknames.Any() 
                ? _context.Nicknames.Max(n => n.NicknameId) + 1 
                : 1;
        }
        
        /// <summary>
        /// Gets player statistics by nickname
        /// </summary>
        /// <param name="playerName">The player's nickname</param>
        /// <returns>Player statistics or null if not found</returns>
        public PlayerStatsRelative GetStatsByNickname(string playerName)
        {
            // Find similar nickname in the database
            var nickname = FindSimilarNicknameInDb(GetAllNicknames(), playerName);
            
            if (nickname == null)
                return null;
                
            // Get the stats associated with this nickname
            var stats = _context.Stats.FirstOrDefault(s => s.NicknameId == nickname.NicknameId);
            
            if (stats == null)
                return null;
                
            // Convert to PlayerStatsRelative and return
            return stats.ToStatsRelative();
        }
        
        /// <summary>
        /// Gets all player statistics from the database
        /// </summary>
        /// <returns>Dictionary of player statistics by nickname</returns>
        public Dictionary<string, PlayerStatsRelative> GetAllStatsWithNicknames()
        {
            var result = new Dictionary<string, PlayerStatsRelative>();
            
            // Get all stats with their nicknames
            var allStats = GetAllStats();
            
            foreach (var stats in allStats)
            {
                if (stats.Nickname != null)
                {
                    result[stats.Nickname.Name] = stats.ToStatsRelative();
                }
            }
            
            return result;
        }

        /// <summary>
        /// Simplified and optimized version of UpdateStats that directly updates the database
        /// for the exact player names provided without using fuzzy matching
        /// </summary>
        /// <param name="playerStats">Dictionary containing player statistics</param>
        public void UpdateStats(Dictionary<string, PlayerStats> playerStats)
        {
            foreach (var (playerName, stats) in playerStats)
            {
                // Convert to relative stats
                var relativeStats = stats.ToRelativeStats();
                
                // Normalize player name
                var normalizedPlayerName = playerName.Trim();
                
                // Try to find the exact nickname in the database
                var existingNickname = _context.Nicknames
                    .FirstOrDefault(n => n.Name == normalizedPlayerName);
                
                if (existingNickname != null)
                {
                    // Update nickname frequency
                    existingNickname.Frequency++;
                    UpdateNickname(existingNickname);
                    
                    // Check if stats exists
                    var statsRecord = _context.Stats
                        .FirstOrDefault(s => s.NicknameId == existingNickname.NicknameId);
                    
                    if (statsRecord != null)
                    {
                        // Update existing stats
                        statsRecord.Hands = relativeStats.Hands;
                        statsRecord.VPIP = (decimal)relativeStats.VPIP;
                        statsRecord.PFR = (decimal)relativeStats.PFR;
                        statsRecord.ThreeBet = (decimal)relativeStats.ThreeBet;
                        statsRecord.FoldToThreeBet = (decimal)relativeStats.FoldToThreeBet;
                        statsRecord.CBetFlop = (decimal)relativeStats.CBetFlop;
                        statsRecord.FoldToCBetFlop = (decimal)relativeStats.FoldToCBetFlop;
                        statsRecord.WTSD = (decimal)relativeStats.WTSD;
                        
                        UpdateStats(statsRecord);
                    }
                    else
                    {
                        // Create new stats with existing nickname
                        var newStats = Stats.FromStatsRelative(existingNickname.NicknameId, relativeStats);
                        AddStats(newStats);
                    }
                }
                else
                {
                    // Create new nickname
                    int newNicknameId = GetNextNicknameId();
                    var newNickname = new Nickname
                    {
                        NicknameId = newNicknameId,
                        Name = normalizedPlayerName,
                        Frequency = 1
                    };
                    
                    AddNickname(newNickname);
                    
                    // Create new stats with new nickname
                    var newStats = Stats.FromStatsRelative(newNicknameId, relativeStats);
                    AddStats(newStats);
                }
            }
        }
    }
}
