using System;
using System.Collections.Generic;
using System.Linq;
using PT.Poker.Model;

namespace PT.Algorithm.Model
{
    public class SimulationParameters : IEquatable<SimulationParameters>
    {
        public CardLayout MyLayout { get; init; }
        public int NumOfPlayers { get; init; }
        public Card[] Board { get; init; }
        
        // Dictionary to store equity values for different hand combinations
        // Key: A string representation of a hand (e.g., "AcKs" for Ace of clubs, King of spades)
        // Value: The equity value (probability of winning) for this hand
        public Dictionary<string[], double> HandEquities { get; set; } = new Dictionary<string[], double>();

        public SimulationParameters()
        {
        }

        public SimulationParameters(RandomSetDefinition randomSetDefinition)
        {
            MyLayout = randomSetDefinition.MyLayout;
            NumOfPlayers = randomSetDefinition.NumOfPlayers;
            Board = randomSetDefinition.Board;
        }

        public bool Equals(SimulationParameters other)
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
            return Equals(obj as SimulationParameters);
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