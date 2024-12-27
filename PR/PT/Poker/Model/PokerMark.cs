using PT.Interfaces;

namespace PT.Poker.Model
{
    public readonly struct PokerMark : IMark
    {
        private PokerLayouts PokerLayout { get; }

        private int PowerOfCards { get; }

        public PokerMark(PokerLayouts pokerLayout, int powerOfCards)
        {
            PokerLayout = pokerLayout;
            PowerOfCards = powerOfCards;
        }

        private int CompareTo(PokerMark other)
        {
            return PokerLayout.CompareTo(other.PokerLayout) switch
            {
                > 0 => 1,
                0 => PowerOfCards.CompareTo(other.PowerOfCards),
                _ => -1
            };
        }

        public int CompareTo(IMark other)
        {
            return CompareTo((PokerMark) other);
        }
    }
}