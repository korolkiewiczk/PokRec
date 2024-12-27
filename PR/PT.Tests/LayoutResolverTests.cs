using PT.Poker.Model;
using PT.Poker.Resolving;

namespace PT.Tests
{
    public class LayoutResolverTests
    {
        [Fact]
        public void HighCard_ShouldReturnHighCard()
        {
            // Arrange: No combinations, just random cards
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.Q),
                new Card(CardColor.Diamonds, CardType.J),
                new Card(CardColor.Spades, CardType.C7),
                new Card(CardColor.Hearts, CardType.C3)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.HighCard, resolver.PokerLayout);
        }

        [Fact]
        public void Pair_ShouldIdentifyPair()
        {
            // Arrange: Pair of Kings
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.K),
                new Card(CardColor.Diamonds, CardType.Q),
                new Card(CardColor.Spades, CardType.C7),
                new Card(CardColor.Hearts, CardType.C4)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.Pair, resolver.PokerLayout);
            Assert.True(resolver.BestLayout.Cards.Count(c => c.CardType == CardType.K) == 2);
        }

        [Fact]
        public void Straight_ShouldIdentifyAceLowStraight()
        {
            // Arrange: Ace-low straight (A, 2, 3, 4, 5)
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Clubs, CardType.C2),
                new Card(CardColor.Diamonds, CardType.C3),
                new Card(CardColor.Spades, CardType.C4),
                new Card(CardColor.Hearts, CardType.C5)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.Straight, resolver.PokerLayout);
            Assert.True(IsAceLowStraight(resolver.BestLayout));
        }

        [Fact]
        public void Flush_ShouldIdentifyFlush()
        {
            // Arrange: Five hearts
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.C2),
                new Card(CardColor.Hearts, CardType.C5),
                new Card(CardColor.Hearts, CardType.C8),
                new Card(CardColor.Hearts, CardType.Q),
                new Card(CardColor.Hearts, CardType.K)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.Flush, resolver.PokerLayout);
            Assert.True(resolver.BestLayout.Cards.All(c => c.CardColor == CardColor.Hearts));
        }

        [Fact]
        public void FourOfAKind_ShouldIdentifyFourOfAKind()
        {
            // Arrange: Four Kings
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.K),
                new Card(CardColor.Diamonds, CardType.K),
                new Card(CardColor.Spades, CardType.K),
                new Card(CardColor.Hearts, CardType.Q)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.FourOfKind, resolver.PokerLayout);
            Assert.Equal(4, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.K));
        }

        [Fact]
        public void FullHouse_ShouldIdentifyFullHouse()
        {
            // Arrange: Full House (Three Aces, Two Kings)
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Clubs, CardType.A),
                new Card(CardColor.Diamonds, CardType.A),
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.K)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.FullHouse, resolver.PokerLayout);
            Assert.True(resolver.BestLayout.Cards.Count(c => c.CardType == CardType.A) == 3);
            Assert.True(resolver.BestLayout.Cards.Count(c => c.CardType == CardType.K) == 2);
        }

        [Fact]
        public void ThreeOfAKind_ShouldIdentifyThreeOfAKind()
        {
            // Arrange: Three Queens
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.Q),
                new Card(CardColor.Clubs, CardType.Q),
                new Card(CardColor.Diamonds, CardType.Q),
                new Card(CardColor.Spades, CardType.C7),
                new Card(CardColor.Hearts, CardType.C4)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.ThreeOfKind, resolver.PokerLayout);
            Assert.Equal(3, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.Q));
        }

        [Fact]
        public void TwoPair_ShouldIdentifyTwoPair()
        {
            // Arrange: Two pairs (Kings and Queens)
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.K),
                new Card(CardColor.Diamonds, CardType.Q),
                new Card(CardColor.Spades, CardType.Q),
                new Card(CardColor.Hearts, CardType.C4)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.TwoPair, resolver.PokerLayout);
            Assert.Equal(2, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.K));
            Assert.Equal(2, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.Q));
        }

        [Fact]
        public void StraightFlush_ShouldIdentifyStraightFlush()
        {
            // Arrange: Straight flush (5, 6, 7, 8, 9 of Hearts)
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.C5),
                new Card(CardColor.Hearts, CardType.C6),
                new Card(CardColor.Hearts, CardType.C7),
                new Card(CardColor.Hearts, CardType.C8),
                new Card(CardColor.Hearts, CardType.C9)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.Poker, resolver.PokerLayout);
            Assert.True(resolver.BestLayout.Cards.All(c => c.CardColor == CardColor.Hearts));
            Assert.True(IsStraight(resolver.BestLayout));
        }

        [Fact]
        public void Pair_ShouldIdentifyCorrectly()
        {
            // Arrange: Pair of Kings
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.K),
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.Pair, resolver.PokerLayout);
            Assert.Equal(2, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.K));
        }

        [Fact]
        public void HighCard_ShouldIdentifyCorrectly()
        {
            // Arrange: Two high cards
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.A),
                new Card(CardColor.Clubs, CardType.K),
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.HighCard, resolver.PokerLayout);
            Assert.Equal(1, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.K));
            Assert.Equal(1, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.A));
            Assert.Equal(0, resolver.BestLayout.Cards.Count(c => c.CardType == CardType.C2));
        }

        [Fact]
        public void HighCard_WithMoreThanFiveCards_ShouldReturnHighCard()
        {
            // Arrange: No combinations, just random cards
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.Q),
                new Card(CardColor.Diamonds, CardType.J),
                new Card(CardColor.Spades, CardType.C7),
                new Card(CardColor.Hearts, CardType.C3),
                new Card(CardColor.Spades, CardType.C2)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.HighCard, resolver.PokerLayout);
        }

        [Fact]
        public void Pair_WithMoreThanFiveCards_ShouldIdentifyPair()
        {
            // Arrange: Pair of Kings
            var cards = new[]
            {
                new Card(CardColor.Hearts, CardType.K),
                new Card(CardColor.Clubs, CardType.K),
                new Card(CardColor.Diamonds, CardType.Q),
                new Card(CardColor.Spades, CardType.C7),
                new Card(CardColor.Hearts, CardType.C4),
                new Card(CardColor.Spades, CardType.C2)
            };
            var layout = new CardLayout(cards);

            // Act
            var resolver = new LayoutResolver(layout);

            // Assert
            Assert.Equal(PokerLayouts.Pair, resolver.PokerLayout);
            Assert.True(resolver.BestLayout.Cards.Count(c => c.CardType == CardType.K) == 2);
        }

        /// <summary>
        /// Helper method to check if the layout is a straight.
        /// </summary>
        private bool IsStraight(CardLayout layout)
        {
            var sortedRanks = layout.Cards.Select(c => (int) c.CardType).OrderBy(x => x).ToList();
            for (int i = 1; i < sortedRanks.Count; i++)
            {
                if (sortedRanks[i] != sortedRanks[i - 1] + 1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Helper method to check Ace-low straight specifically.
        /// </summary>
        private bool IsAceLowStraight(CardLayout layout)
        {
            var sortedRanks = layout.Cards.Select(c => (int) c.CardType).OrderBy(x => x).ToList();
            return sortedRanks.SequenceEqual([(int) CardType.C2, (int) CardType.C3, (int) CardType.C4, (int) CardType.C5, (int) CardType.A
            ]);
        }
    }
}

