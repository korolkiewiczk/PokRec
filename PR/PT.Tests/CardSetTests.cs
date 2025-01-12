using PT.Poker.Model;

namespace PT.Tests;

public class CardSetTests
{
    [Fact]
    public void Generate_ShouldThrowException_WhenUserLayoutHasMoreThanTwoCards()
    {
        var cardSet = new CardSet();
        var arg = new RandomSetDefinition
        {
            MyLayout = new CardLayout(new Card[3])
        };

        Assert.Throws<ArgumentNullException>(() => cardSet.Generate(arg));
    }

    [Fact]
    public void Generate_ShouldGenerateCorrectNumberOfCards()
    {
        var cardSet = new CardSet();
        var arg = new RandomSetDefinition
        {
            MyLayout = new CardLayout(new Card[2]),
            Board = [new Card(CardColor.Hearts, CardType.A)],
            NumOfPlayers = 3
        };

        cardSet.Generate(arg);

        Assert.Equal(3, cardSet.CardLayouts.Length);
    }

    [Fact]
    public void IsWinning_ShouldReturnTrue_WhenMyLayoutIsBest()
    {
        var myLayout = new CardLayout([
            new Card(CardColor.Hearts, CardType.A),
            new Card(CardColor.Spades, CardType.K)
        ]);
        var otherLayout = new CardLayout([
            new Card(CardColor.Diamonds, CardType.Q),
            new Card(CardColor.Clubs, CardType.J)
        ]);
        var cardSet = new CardSet([myLayout, otherLayout]);

        Assert.True(cardSet.IsWinning);
    }

    [Fact]
    public void IsWinning_ShouldReturnTrue_WhenMyLayoutIsBest_WithComplexDeck()
    {
        var myLayout = new CardLayout([
            new Card(CardColor.Hearts, CardType.A),
            new Card(CardColor.Spades, CardType.A),
            new Card(CardColor.Diamonds, CardType.Q),
            new Card(CardColor.Clubs, CardType.J),
            new Card(CardColor.Hearts, CardType.C10)
        ]);
        var otherLayout1 = new CardLayout([
            new Card(CardColor.Diamonds, CardType.A),
            new Card(CardColor.Clubs, CardType.K),
            new Card(CardColor.Spades, CardType.Q),
            new Card(CardColor.Hearts, CardType.J),
            new Card(CardColor.Diamonds, CardType.C9)
        ]);
        var otherLayout2 = new CardLayout([
            new Card(CardColor.Clubs, CardType.A),
            new Card(CardColor.Hearts, CardType.K),
            new Card(CardColor.Diamonds, CardType.Q),
            new Card(CardColor.Spades, CardType.J),
            new Card(CardColor.Clubs, CardType.C8)
        ]);
        var cardSet = new CardSet([myLayout, otherLayout1, otherLayout2]);

        Assert.True(cardSet.IsWinning);
    }
    
    [Fact]
    public void IsLoosing_ShouldReturnTrue_WhenMyLayoutIsNotBest_WithComplexDeck()
    {
        var myLayout = new CardLayout([
            new Card(CardColor.Hearts, CardType.A),
            new Card(CardColor.Spades, CardType.A),
            new Card(CardColor.Diamonds, CardType.Q),
            new Card(CardColor.Clubs, CardType.J),
            new Card(CardColor.Hearts, CardType.C10)
        ]);
        var otherLayout1 = new CardLayout([
            new Card(CardColor.Diamonds, CardType.A),
            new Card(CardColor.Clubs, CardType.K),
            new Card(CardColor.Spades, CardType.Q),
            new Card(CardColor.Hearts, CardType.J),
            new Card(CardColor.Diamonds, CardType.C10)
        ]);
        var otherLayout2 = new CardLayout([
            new Card(CardColor.Clubs, CardType.A),
            new Card(CardColor.Hearts, CardType.K),
            new Card(CardColor.Diamonds, CardType.Q),
            new Card(CardColor.Spades, CardType.J),
            new Card(CardColor.Clubs, CardType.C8)
        ]);
        var cardSet = new CardSet([myLayout, otherLayout1, otherLayout2]);

        Assert.True(cardSet.IsLoosing);
    }

    [Fact]
    public void IsLoosing_ShouldReturnTrue_WhenMyLayoutIsWorst()
    {
        var otherLayout = new CardLayout([
            new Card(CardColor.Hearts, CardType.A),
            new Card(CardColor.Spades, CardType.K)
        ]);
        var myLayout = new CardLayout([
            new Card(CardColor.Diamonds, CardType.Q),
            new Card(CardColor.Clubs, CardType.J)
        ]);
        var cardSet = new CardSet([myLayout, otherLayout]);

        Assert.True(cardSet.IsLoosing);
    }
}