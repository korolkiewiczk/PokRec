namespace PT.Poker.Model
{
    public struct RandomSetDefinition
    {
        /// <summary>
        /// Including my layout
        /// </summary>
        public int NumOfPlayers { get; init; }

        /// <summary>
        /// Should contain only 2 cards
        /// </summary>
        public CardLayout MyLayout { get; init; }

        /// <summary>
        /// Flop, Turn, River cards (or empty)
        /// </summary>
        public Card[] Board { get; init; }
    }
}
