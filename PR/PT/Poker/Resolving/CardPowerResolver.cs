using System.Linq;
using PT.Poker.Model;

namespace PT.Poker.Resolving
{
    public class CardPowerResolver
    {
        private readonly CardLayout _layout;

        public CardPowerResolver(CardLayout layout)
        {
            _layout = layout;
        }

        public int Resolve()
        {
            return _layout.Cards.Sum(x => x.Power);
        }
    }
}
