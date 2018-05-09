using System.Collections.Generic;
using System.Linq;
using Common;

namespace Game.Games
{
    public class PokerBoardSettingsParser
    {
        private readonly List<KeyValuePair<string, string>> _settings;

        public PokerBoardSettingsParser(Board board)
        {
            _settings = board.Settings?? new List<KeyValuePair<string, string>>();
        }

        public int Players => int.Parse(_settings.FirstOrDefault(x => x.Key == nameof(Players)).Value ?? "9");
    }
}