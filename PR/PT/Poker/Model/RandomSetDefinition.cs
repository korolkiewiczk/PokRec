namespace PT.Poker.Model
{
    public struct RandomSetDefinition
    {
        /// <summary>
        /// Including my layout
        /// </summary>
        public int NumOfPlayers { get; set; }

        /// <summary>
        /// Should contain only 2 cards
        /// </summary>
        public CardLayout MyLayout { get; set; }

        /// <summary>
        /// Flop, Turn, River cards (or empty)
        /// </summary>
        public Card[] Board { get; set; }
    }
}
