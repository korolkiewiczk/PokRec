using PT.Interfaces;

namespace PT.Poker.Model
{
    public struct PokerMark : IMark
    {
        private readonly PokerLayouts _pokerLayout;
        private readonly int _powerOfCards;
        private readonly CardLayout _bestLayout;

        public PokerLayouts PokerLayout
        {
            get { return _pokerLayout; }
        }

        public int PowerOfCards
        {
            get { return _powerOfCards; }
        }

        public CardLayout BestLayout
        {
            get { return _bestLayout; }
        }

        public PokerMark(PokerLayouts pokerLayout, int powerOfCards, CardLayout bestLayout)
        {
            _pokerLayout = pokerLayout;
            _powerOfCards = powerOfCards;
            _bestLayout = bestLayout;
        }

        private int CompareTo(PokerMark other)
        {
            if (PokerLayout > other.PokerLayout)
            {
                return 1;
            }
            if (PokerLayout == other.PokerLayout)
            {
                return PowerOfCards.CompareTo(other.PowerOfCards);
            }
            return -1;
        }

        public int CompareTo(IMark other)
        {
            return CompareTo((PokerMark)other);
        }
    }
}
