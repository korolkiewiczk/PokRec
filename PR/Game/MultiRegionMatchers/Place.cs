using System.Collections.Generic;
using System.Linq;

namespace Game.MultiRegionMatchers
{
    public class Place
    {
        public Place()
        {
            IsSet = new HashSet<int>();
        }

        public void Add(int pos) => IsSet.Add(pos);

        public int Pos => IsSet.FirstOrDefault() + 1;

        public int Count => IsSet.Count;
        
        protected HashSet<int> IsSet { get; }
    }
}