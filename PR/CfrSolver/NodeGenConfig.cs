namespace CfrSolver
{
    public class NodeGenConfig
    {
        /// <summary>
        /// Possible raises. Can be absolute if <see cref="RelativeBetting" /> is set to false or relative to current pot in percents/>
        /// </summary>
        public int[][] PossibleRaises { get; set; }
        
        /// <summary>
        /// Number of possible reraises in given round
        /// </summary>
        public int ReraiseAmount { get; set; }

        /// <summary>
        /// Number of players. Right now the only valid number is 2
        /// </summary>
        public int NumPlayers { get; set; }

        /// <summary>
        /// Small blind value
        /// </summary>
        public int SbValue { get; set; }

        /// <summary>
        /// Big blind value
        /// </summary>
        public int BbValue { get; set; }

        /// <summary>
        /// Total bankroll of both players (we assume they are equal)
        /// </summary>
        public int Bankroll { get; set; }

        /// <summary>
        /// If set to true, possible raises contains percent values of current pot to be bet at given node
        /// </summary>
        public bool RelativeBetting { get; set; }
    }
}
