using CfrSolver.Model;
using PT.Poker.Model;

namespace CfrSolver.Interfaces
{
    public interface IBoardGenerator
    {
        BoardInfo GenerateBoardAbstraction();
        
        /// <summary>
        /// Generates board abstraction using only the provided cards.
        /// Only the player's hand is evaluated up to the stage provided.
        /// Opponent hand and WinningPlayer are left undefined.
        /// </summary>
        /// <param name="playerHand">Player's hole cards (exactly 2 cards required).</param>
        /// <param name="flop">Optional flop cards (3 cards if provided).</param>
        /// <param name="turn">Optional turn card (requires a complete flop).</param>
        /// <param name="river">Optional river card (requires complete flop and turn).</param>
        BoardInfo GenerateBoardAbstraction(
            Card[] playerHand,
            Card[] flop = null,
            Card[] turn = null,
            Card[] river = null);

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
        BoardInfo GenerateBoardAbstractionRandom(
            Card[] playerHand,
            Card[] flop = null,
            Card[] turn = null,
            Card[] river = null,
            Card[] oppHand = null);
    }
}