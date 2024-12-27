using System;
using System.Linq;

namespace PT.Poker.Model
{
    public class RandomSetDefinition : IEquatable<RandomSetDefinition>
    {
        public CardLayout MyLayout { get; init; }
        public int NumOfPlayers { get; init; }
        public Card[] Board { get; init; }

        public bool Equals(RandomSetDefinition other)
        {
            if (other == null) return false;
            if (NumOfPlayers != other.NumOfPlayers) return false;
            if (!MyLayout.Equals(other.MyLayout)) return false;
            if (Board.Length != other.Board.Length) return false;
            for (int i = 0; i < Board.Length; i++)
            {
                if (!Board[i].Equals(other.Board[i])) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RandomSetDefinition);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + NumOfPlayers.GetHashCode();
            hash = hash * 23 + MyLayout.GetHashCode();
            return Board.Aggregate(hash, (current, card) => current * 23 + card.GetHashCode());
        }
    }
}