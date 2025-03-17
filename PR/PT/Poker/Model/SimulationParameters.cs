using System.Collections.Generic;

namespace PT.Poker.Model
{
    public class SimulationParameters : RandomSetDefinition
    {
        public Dictionary<string, double> HandEquities { get; }

        public SimulationParameters(RandomSetDefinition randomSetDefinition, Dictionary<string, double> handEquities)
        {
            MyLayout = randomSetDefinition.MyLayout;
            NumOfPlayers = randomSetDefinition.NumOfPlayers;
            Board = randomSetDefinition.Board;
            HandEquities = handEquities;
        }
    }
} 