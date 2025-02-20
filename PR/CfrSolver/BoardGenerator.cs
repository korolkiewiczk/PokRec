using CfrSolver.Interfaces;
using CfrSolver.Model;
using CfrSolver.Utils;
using PT.Poker.Model;

namespace CfrSolver
{
    public class BoardGenerator : IBoardGenerator
    {
        private Card[] _baseCards;
        private readonly int _maxHandResolution;
        private readonly bool _twoPlayer;
        private readonly Random _random = new();

        public BoardGenerator(int maxHandResolution, bool twoPlayer = false)
        {
            _maxHandResolution = maxHandResolution;
            _twoPlayer = twoPlayer;
        }

        /// <summary>
        /// Existing full random board generator
        /// </summary>
        public BoardInfo GenerateBoardAbstraction()
        {
            Card[] cards = GetShuffledCards();

            // Player gets the first 7 cards.
            CardLayout playerDeck = new CardLayout(cards.Take(7).ToArray());
            // Opponent deck is built as in the original code.
            CardLayout oppDeck = new CardLayout(cards.Skip(7).Take(2)
                                                .Union(cards.Skip(2).Take(5))
                                                .ToArray());
            CardSet cardSet = new CardSet([playerDeck, oppDeck]);
            BoardInfo boardInfo = new BoardInfo();

            if (cardSet.IsWinning)
                boardInfo.WinningPlayer = 0;
            else if (cardSet.IsLoosing)
                boardInfo.WinningPlayer = 1;
            else
                boardInfo.WinningPlayer = -1;

            int hand = 0;
            Card[] playerHand = playerDeck.Cards.Take(2).ToArray();
            hand += EvaluatePreflop(playerHand);

            Card[] playerFlop = playerDeck.Cards.Take(5).ToArray();
            hand += EvaluateFlop(playerFlop) * _maxHandResolution;

            Card[] playerTurn = playerDeck.Cards.Take(6).ToArray();
            hand += EvaluateTurn(playerTurn) * _maxHandResolution * _maxHandResolution;

            Card[] playerRiver = playerDeck.Cards.Take(7).ToArray();
            hand += EvaluateRiver(playerRiver) * _maxHandResolution * _maxHandResolution * _maxHandResolution;

            boardInfo.Hand = hand;

            if (_twoPlayer)
            {
                int hand2 = 0;
                Card[] oppHand = oppDeck.Cards.Take(2).ToArray();
                hand2 += EvaluatePreflop(oppHand);

                Card[] oppFlop = oppDeck.Cards.Take(5).ToArray();
                hand2 += EvaluateFlop(oppFlop) * _maxHandResolution;

                Card[] oppTurn = oppDeck.Cards.Take(6).ToArray();
                hand2 += EvaluateTurn(oppTurn) * _maxHandResolution * _maxHandResolution;

                Card[] oppRiver = oppDeck.Cards.Take(7).ToArray();
                hand2 += EvaluateRiver(oppRiver) * _maxHandResolution * _maxHandResolution * _maxHandResolution;

                boardInfo.OppHand = hand2;
            }
            return boardInfo;
        }

        /// <summary>
        /// Generates board abstraction using only the provided cards.
        /// Only the player's hand is evaluated up to the stage provided.
        /// Opponent hand and WinningPlayer are left undefined.
        /// </summary>
        /// <param name="playerHand">Player's hole cards (exactly 2 cards required).</param>
        /// <param name="flop">Optional flop cards (3 cards if provided).</param>
        /// <param name="turn">Optional turn card (requires a complete flop).</param>
        /// <param name="river">Optional river card (requires complete flop and turn).</param>
        public BoardInfo GenerateBoardAbstraction(
            Card[] playerHand,
            Card[] flop = null,
            Card[] turn = null,
            Card[] river = null)
        {
            if (playerHand == null || playerHand.Length != 2)
                throw new ArgumentException("Player hand must contain exactly 2 cards.");

            BoardInfo boardInfo = new BoardInfo();
            int handValue = 0;

            // Always evaluate preflop.
            handValue += EvaluatePreflop(playerHand);

            if (flop != null)
            {
                if (flop.Length != 3)
                    throw new ArgumentException("Flop must contain exactly 3 cards.");
                Card[] handWithFlop = playerHand.Concat(flop).ToArray(); // 5 cards
                handValue += EvaluateFlop(handWithFlop) * _maxHandResolution;
            }
            if (turn != null)
            {
                if (flop == null)
                    throw new ArgumentException("Turn provided requires a complete flop (3 cards).");
                Card[] handWithTurn = playerHand.Concat(flop).Concat(turn).ToArray(); // 6 cards
                handValue += EvaluateTurn(handWithTurn) * _maxHandResolution * _maxHandResolution;
            }
            if (river != null)
            {
                if (flop == null || turn == null)
                    throw new ArgumentException("River provided requires a complete flop (3 cards) and turn card.");
                Card[] handWithRiver = playerHand.Concat(flop).Concat(turn).Concat(river).ToArray(); // 7 cards
                handValue += EvaluateRiver(handWithRiver) * _maxHandResolution * _maxHandResolution * _maxHandResolution;
            }
            boardInfo.Hand = handValue;
            // For non-random version, OppHand and WinningPlayer remain unset.
            return boardInfo;
        }

        /// <summary>
        /// Generates a complete board abstraction.
        /// Missing community cards (flop, turn, river) and opponent's hand are randomly generated.
        /// Both player's and opponent's hands are fully evaluated.
        /// </summary>
        /// <param name="playerHand">Player's hole cards (exactly 2 cards required).</param>
        /// <param name="flop">Optional flop cards (3 cards if provided).</param>
        /// <param name="turn">Optional turn card.</param>
        /// <param name="river">Optional river card.</param>
        /// <param name="oppHand">Optional opponent hole cards (2 cards if provided).</param>
        public BoardInfo GenerateBoardAbstractionRandom(
            Card[] playerHand,
            Card[] flop = null,
            Card[] turn = null,
            Card[] river = null,
            Card[] oppHand = null)
        {
            if (playerHand == null || playerHand.Length != 2)
                throw new ArgumentException("Player hand must contain exactly 2 cards.");

            // Record provided cards.
            HashSet<Card> usedCards = new HashSet<Card>(playerHand);
            if (flop != null)
            {
                if (flop.Length != 3)
                    throw new ArgumentException("Flop must contain exactly 3 cards.");
                foreach (var card in flop)
                    usedCards.Add(card);
            }
            if (turn != null)
                usedCards.Add(turn[0]);
            if (river != null)
                usedCards.Add(river[0]);
            if (oppHand != null)
            {
                if (oppHand.Length != 2)
                    throw new ArgumentException("Opponent hand must contain exactly 2 cards.");
                foreach (var card in oppHand)
                    usedCards.Add(card);
            }

            // Get remaining cards from deck.
            List<Card> remaining = GetRemainingCards(usedCards);

            // Generate missing community cards.
            if (flop == null)
            {
                flop = remaining.Take(3).ToArray();
                usedCards.UnionWith(flop);
                remaining = GetRemainingCards(usedCards);
            }
            if (turn == null)
            {
                turn = [remaining.First()];
                usedCards.Add(turn[0]);
                remaining = GetRemainingCards(usedCards);
            }
            if (river == null)
            {
                river = [remaining.First()];
                usedCards.Add(river[0]);
                remaining = GetRemainingCards(usedCards);
            }
            // Generate opponent hand if in two-player mode.
            if (_twoPlayer)
            {
                if (oppHand == null)
                {
                    oppHand = remaining.Take(2).ToArray();
                    usedCards.UnionWith(oppHand);
                    remaining = GetRemainingCards(usedCards);
                }
            }
            else
            {
                // In single-player mode, leave opponent hand empty.
                oppHand = new Card[0];
            }

            // Evaluate player's full hand (hole cards + community cards).
            int playerValue = 0;
            Card[] playerFullHand = playerHand
                .Concat(flop)
                .Concat(turn)
                .Concat(river)
                .ToArray();

            playerValue += EvaluatePreflop(playerHand);
            playerValue += EvaluateFlop(playerHand.Concat(flop).ToArray()) * _maxHandResolution;
            playerValue += EvaluateTurn(playerHand.Concat(flop).Concat(turn).ToArray()) * _maxHandResolution * _maxHandResolution;
            playerValue += EvaluateRiver(playerFullHand) * _maxHandResolution * _maxHandResolution * _maxHandResolution;

            int oppValue = 0;
            if (_twoPlayer)
            {
                // Opponent uses its own hole cards plus the same community board.
                oppValue += EvaluatePreflop(oppHand);
                oppValue += EvaluateFlop(oppHand.Concat(flop).ToArray()) * _maxHandResolution;
                oppValue += EvaluateTurn(oppHand.Concat(flop).Concat(turn).ToArray()) * _maxHandResolution * _maxHandResolution;
                oppValue += EvaluateRiver(oppHand.Concat(flop).Concat(turn).Concat(river).ToArray()) * _maxHandResolution * _maxHandResolution * _maxHandResolution;
            }

            BoardInfo boardInfo = new BoardInfo();
            boardInfo.Hand = playerValue;
            boardInfo.OppHand = oppValue;
            if (_twoPlayer)
            {
                boardInfo.WinningPlayer = playerValue > oppValue ? 0 : (playerValue < oppValue ? 1 : -1);
            }
            else
            {
                boardInfo.WinningPlayer = -1;
            }
            return boardInfo;
        }

        private int EvaluatePreflop(Card[] playerHand)
        {
            if (playerHand[0].CardType == playerHand[1].CardType)
                return playerHand[0].CardType > CardType.C10 ? ((int)playerHand[0].CardType - (int)CardType.J + 4) : 3;
            if ((playerHand[0].CardType > CardType.C10 && playerHand[1].CardType > CardType.C10) ||
                (playerHand[0].CardType > CardType.C7 && playerHand[1].CardType > CardType.C7 &&
                 playerHand[0].CardColor == playerHand[1].CardColor))
                return 2;
            if (playerHand[0].CardType > CardType.C7 && playerHand[1].CardType > CardType.C7)
                return 1;
            return 0;
        }

        private static int GetHandBucket(PokerMark mark)
        {
            return Math.Min((int)mark.PokerLayout - 1, 7) / 2;
        }

        private int EvaluateFlop(Card[] cards)
        {
            var cardLayout = new CardLayout(cards);
            PokerMark mark = (PokerMark)cardLayout.GetMark();
            return GetHandBucket(mark);
        }

        private int EvaluateTurn(Card[] cards)
        {
            var cardLayout = new CardLayout(cards);
            PokerMark mark = (PokerMark)cardLayout.GetMark();
            return GetHandBucket(mark);
        }

        private int EvaluateRiver(Card[] cards)
        {
            var cardLayout = new CardLayout(cards);
            PokerMark mark = (PokerMark)cardLayout.GetMark();
            return GetHandBucket(mark);
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

        private List<Card> GetRemainingCards(HashSet<Card> usedCards)
        {
            if (_baseCards == null)
            {
                // Ensure the base deck is built.
                GetShuffledCards();
            }
            List<Card> remaining = _baseCards.Where(card => !usedCards.Contains(card)).ToList();
            // Shuffle the remaining cards.
            remaining = remaining.OrderBy(x => _random.Next()).ToList();
            return remaining;
        }
    }
}
