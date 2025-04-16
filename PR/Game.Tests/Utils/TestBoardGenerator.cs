using CfrSolver.Interfaces;
using CfrSolver.Model;
using PT.Poker.Model;

namespace Game.Tests;

internal class TestBoardGenerator : IBoardGenerator
{
    public BoardInfo GenerateBoardAbstraction()
    {
        return new BoardInfo
        {
            Hand = 1,
            OppHand = 0,
            WinningPlayer = 0
        };
    }

    public BoardInfo GenerateBoardAbstraction(Card[] playerHoleCards, Card[] flopCards, Card[] turnCards,
        Card[] riverCards)
    {
        return new BoardInfo
        {
            Hand = 1,
            OppHand = 0,
            WinningPlayer = 0
        };
    }

    public BoardInfo GenerateBoardAbstractionRandom(Card[] playerHoleCards, Card[] flopCards, Card[] turnCards,
        Card[] riverCards, Card[] opponentHoleCards)
    {
        return new BoardInfo
        {
            Hand = 1,
            OppHand = 0,
            WinningPlayer = 0
        };
    }
}