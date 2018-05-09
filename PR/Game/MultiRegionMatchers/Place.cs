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

        public int Pos => IsSet.FirstOrDefault();

        public int Count => IsSet.Count;
        
        private HashSet<int> IsSet { get; }
    }
}