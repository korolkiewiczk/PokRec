﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common;
using Game.Common;
using Game.Games.TexasHoldem.Solving;
using Game.Games.TexasHoldem.Utils;
using Game.Interfaces;
using Game.MultiRegionMatchers;
using Game.RegionMatchers;
using PT.Poker.Model;

namespace Game.Games.TexasHoldem.Presenters
{
    public class PokerPresenter
    {
        private readonly Poker _poker;

        public PokerPresenter(Poker poker)
        {
            _poker = poker;
        }

        public void Show(Environment e)
        {
            var (reconResults, matchResult, monteCarloResult, bestLayout, pokerPosition, _, phase, isCorrectPot, evResult) = _poker.Solve();
            var presenters = _poker.GetPresenters();

            presenters[nameof(PlayerCards)].Present(reconResults.PlayerResult, e);
            presenters[nameof(Flop)].Present(reconResults.FlopResult, e);
            presenters[nameof(Turn)].Present(reconResults.TurnResult, e);
            presenters[nameof(River)].Present(reconResults.RiverResult, e);

            PresentResults(presenters[nameof(Position)], reconResults.PositionResults, e);
            PresentResults(presenters[nameof(Opponent)], reconResults.OpponentResults, e);
            PresentResults(presenters[nameof(Stack)], reconResults.StackResults, e);
            PresentResults(presenters[nameof(Nickname)], reconResults.NicknameResults, e);
            presenters[nameof(Decision)].Present(reconResults.DecisionResult, e);
            presenters[nameof(Pot)].Present(reconResults.PotResult, e);

            //if (matchResult.PlayerCards.Any())
            {
                int countPlayers = matchResult.Opponent.Count + 1; // +1 for the player

                var playerCardsStr = string.Join(" ", matchResult.PlayerCards.Select(c => c.ToString()));
                var flopCardsStr = string.Join(" ", matchResult.Flop.Select(c => c.ToString()));
                var turnCardsStr = string.Join(" ", matchResult.Turn.Select(c => c.ToString()));
                var riverCardsStr = string.Join(" ", matchResult.River.Select(c => c.ToString()));

                void DrawColoredString(string text, Color color, ref int x, int y, Graphics g, Font font)
                {
                    g.DrawString(text, font, new SolidBrush(color), x, y);
                    x += (int) g.MeasureString(text + " ", font).Width;
                }

                var font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);
                var smallFont = new Font(FontFamily.GenericMonospace, 5, FontStyle.Regular);
                var lineHeight = font.GetHeight(e.Graphics);
                // First line - Players count, Position and Phase
                var x = 10;
                var y = 10;

                var positionText = pokerPosition?.ToDisplayString();
                DrawColoredString($"Players = {countPlayers} Position = {positionText} Phase = {phase}", Color.Black, ref x, y,
                    e.Graphics, font);
                // Second line - Layout and cards
                x = 10;
                y += (int) lineHeight;
                const string prefix2 = "Layout = ";
                DrawColoredString(prefix2, Color.Black, ref x, y, e.Graphics, font);
                DrawColoredString(playerCardsStr, Color.Black, ref x, y, e.Graphics, font);
                DrawColoredString(flopCardsStr, Color.FromArgb(205, 127, 50), ref x, y, e.Graphics, font); // Bronze
                DrawColoredString(turnCardsStr, Color.Orange, ref x, y, e.Graphics, font);
                DrawColoredString(riverCardsStr, Color.Violet, ref x, y, e.Graphics, font);

                // Display the best layout
                x = 10;
                y += (int) lineHeight;
                if (bestLayout != null)
                {
                    DrawColoredString($"Best Poker Hand: {bestLayout.Value.ToDisplayString()}",
                        bestLayout.Value.ToColor(), ref x, y, e.Graphics, font);
                }

                // Second line
                x = 10;
                y += (int) lineHeight;
                if (monteCarloResult != null)
                {
                    e.Graphics.DrawString(
                        $"WIN {monteCarloResult.Value.Better * 100:F1}% - TIE {monteCarloResult.Value.Exact * 100:F1}% - LOSE {monteCarloResult.Value.Smaller * 100:F1}%",
                        font, new SolidBrush(Color.Black), x, y);
                }

                if (matchResult.IsPlayerDecision)
                {
                    x = 10;
                    y += (int) lineHeight;
                    e.Graphics.DrawString("Your Turn to Act!",
                        font, new SolidBrush(Color.Red), x, y);
                    if (evResult != null)
                    {
                        x = 10;
                        y += (int) lineHeight;
                        e.Graphics.DrawString(evResult.ToString(),
                            font, new SolidBrush(Color.Red), x, y);
                    }
                }

                if (matchResult.Pot != 0)
                {
                    x = 10;
                    y += (int) lineHeight;
                    e.Graphics.DrawString($"Pot: ${matchResult.Pot}", font, new SolidBrush(Color.Black), x, y);
                }

                // Add this after all other text drawing
                if (matchResult.IsPlayerDecision && !isCorrectPot)
                {
                    x = 10;
                    y += (int)lineHeight;
                    e.Graphics.DrawString("Error: Wrong computed POT", font, new SolidBrush(Color.Red), x, y);
                }

                // Display game actions
                if (_poker.GameActions.Any() && _poker.StartingBets != null)
                {
                    x = 10;
                    y += (int)lineHeight;
                    e.Graphics.DrawString($"A={_poker.StartingBets.Ante}, SB={_poker.StartingBets.SmallBlind} BB={_poker.StartingBets.BigBlind}", font, new SolidBrush(Color.Black), x, y);
                    
                    y += (int)lineHeight;
                    e.Graphics.DrawString("Actions:", font, new SolidBrush(Color.Black), x, y);

                    foreach (var action in _poker.GameActions)
                    {
                        x = 10;
                        y += (int)lineHeight;
                        e.Graphics.DrawString(action.ToString(), font, new SolidBrush(Color.Black), x, y);
                    }
                }

                y += 30;
                e.Graphics.DrawString(_poker.PlayerStats.ToDebugString(), smallFont, new SolidBrush(Color.Black), x, y);
            }
        }

        private static void PresentResults(IResultPresenter presenter, IEnumerable<ReconResult> results, Environment e)
        {
            foreach (var result in results)
            {
                presenter.Present(result, e);
            }
        }
    }
}