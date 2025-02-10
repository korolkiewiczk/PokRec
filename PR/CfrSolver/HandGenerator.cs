using CfrSolver.Interfaces;
using CfrSolver.Model;
using CfrSolver.Utils;
using PT.Poker.Model;

namespace CfrSolver
{
    public class HandGenerator : IHandGenerator
    {
        private Card[] _baseCards;
        private readonly int _maxHandResolution;
        private readonly bool _twoPlayer;
        private readonly Random _random = new Random();

        public HandGenerator(int maxHandResolution, bool twoPlayer = false)
        {
            _maxHandResolution = maxHandResolution;
            _twoPlayer = twoPlayer;
        }

        public HandInfo GenerateRandomHand()
        {
            HandInfo handInfo = new HandInfo();

            Card[] cards = GetShuffledCards();
            CardLayout playerDeck = new CardLayout(cards.Take(7).ToArray());
            CardLayout oppDeck = new CardLayout(cards.Skip(7).Take(2).Union(cards.Skip(2).Take(5)).ToArray());
            CardSet cardSet = new CardSet([playerDeck, oppDeck]);

            if (cardSet.IsWinning) handInfo.WinningPlayer = 0;
            else if (cardSet.IsLoosing) handInfo.WinningPlayer = 1;
            else handInfo.WinningPlayer = -1;

            //PreFlop,Flop,Turn,River hand evaluation
            int hand = 0;

            Card[] playerHand = playerDeck.Cards.Take(2).ToArray();
            hand += EvaluatePreflop(playerHand);

            Card[] playerFlop = playerDeck.Cards.Take(5).ToArray();
            hand += EvaluateFlop(playerFlop) * _maxHandResolution;

            Card[] playerTurn = playerDeck.Cards.Take(6).ToArray();
            hand += EvaluateTurn(playerTurn) * _maxHandResolution * _maxHandResolution;

            Card[] playerRiver = playerDeck.Cards.Take(7).ToArray();
            hand += EvaluateRiver(playerRiver) * _maxHandResolution * _maxHandResolution * _maxHandResolution;

            int hand2 = 0;
            if (_twoPlayer)
            {
                Card[] oppHand = oppDeck.Cards.Take(2).ToArray();
                hand2 += EvaluatePreflop(oppHand);

                Card[] oppFlop = oppDeck.Cards.Take(5).ToArray();
                hand2 += EvaluateFlop(oppFlop) * _maxHandResolution;

                Card[] oppTurn = oppDeck.Cards.Take(6).ToArray();
                hand2 += EvaluateTurn(oppTurn) * _maxHandResolution * _maxHandResolution;

                Card[] oppRiver = oppDeck.Cards.Take(7).ToArray();
                hand2 += EvaluateRiver(oppRiver) * _maxHandResolution * _maxHandResolution * _maxHandResolution;
            }

            handInfo.Hand = hand;
            handInfo.OppHand = hand2;
            return handInfo;
        }

        private int EvaluatePreflop(Card[] playerHand)
        {
            if (playerHand[0].CardType == playerHand[1].CardType) return 3;
            if (playerHand[0].CardType > CardType.C10 && playerHand[1].CardType > CardType.C10 ||
                (playerHand[0].CardType > CardType.C7 && playerHand[1].CardType > CardType.C7 && playerHand[0].CardColor == playerHand[1].CardColor)) return 2;
            if (playerHand[0].CardType > CardType.C7 && playerHand[1].CardType > CardType.C7) return 1;
            return 0;
        }

        private int EvaluateFlop(Card[] playerFlop)
        {
            var cardLayout = new CardLayout(playerFlop);
            PokerMark mark = (PokerMark)cardLayout.GetMark();

            return Math.Min((int)mark.PokerLayout / 2, 3);
        }

        private int EvaluateTurn(Card[] playerFlop)
        {
            var cardLayout = new CardLayout(playerFlop);
            PokerMark mark = (PokerMark)cardLayout.GetMark();

            return Math.Min((int)mark.PokerLayout / 2, 3);
        }

        private int EvaluateRiver(Card[] playerFlop)
        {
            var cardLayout = new CardLayout(playerFlop);
            PokerMark mark = (PokerMark)cardLayout.GetMark();

            return Math.Min((int)mark.PokerLayout / 2, 3);
        }

        private Card[] GetShuffledCards()
        {
            if (_baseCards == null)
            {
                _baseCards = new Card[52];

                for (int i = 0; i < 13; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        _baseCards[i * 4 + j] = new Card((CardColor)j, (CardType)i);
                    }
                }
            }

            Card[] myCards = new Card[52];
            Array.Copy(_baseCards, myCards, 52);

            CardsShuffler.ShuffleCards(myCards, _random);

            return myCards;
        }
    }
}
