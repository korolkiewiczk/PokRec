using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public record Place
    {
        public Place()
        {
            IsSet = new HashSet<int>();
        }

        public void Add(int pos) => IsSet.Add(pos);

        public List<bool> Places(int n)
        {
            var result = new List<bool>(new bool[n]);
            foreach (var pos in IsSet)
            {
                if (pos >= 0 && pos < n)
                {
                    result[pos] = true;
                }
            }

            return result;
        }

        public int Pos => IsSet.FirstOrDefault() + 1;

        public int Count => IsSet.Count;

        protected HashSet<int> IsSet { get; }

        public virtual bool Equals(Place other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // Compare sets by their elements:
            return IsSet.SetEquals(other.IsSet);
        }

        public override int GetHashCode()
        {
            // For best practice, combine hash codes of all elements
            // Or something simpler if the set can be large
            unchecked
            {
                var hash = 17;
                foreach (var elem in IsSet.OrderBy(e => e))
                {
                    hash = hash * 31 + elem.GetHashCode();
                }

                return hash;
            }
        }
    }
}