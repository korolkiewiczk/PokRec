using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace Common
{
    public class Board
    {
        public string Name { get; set; }
        public Rectangle Rect { get; set; }

        public List<KeyValuePair<string,string>> Settings { get; set; }

        public override string ToString()
        {
            return Id;
        }

        [JsonIgnore]
        public string Id => $"{Rect.Width}X{Rect.Height}_{Name}";
    }
}
