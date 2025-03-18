using System.Linq;
using PT.Poker.Model;

namespace PT.Poker.Resolving
{
    public static class CardPowerResolver
    {
        public static int Resolve(CardLayout layout)
        {
            return layout.Cards.Sum(x => x.Power);
        }
    }
}
