using System.Collections.Generic;
using System.Linq;
using Game.Common.Model;

namespace Game.Utils
{
    public static class PokerActionExtraction
    {
        private const int HeroPosition = 1;

        /// <summary>
        /// Determines the position for hero (player 1) versus the given opponent (playerNum).
        /// </summary>
        public static int DeterminePosition(List<PlayerAction> actions, int playerNum)
        {
            var noneActions = actions
                .Where(a => (a.PlayerIndex == HeroPosition || a.PlayerIndex == playerNum) && a.Phase == PokerPhase.None)
                .ToList();

            if (!noneActions.Any())
            {
                var filtered = actions
                    .Where(a => a.PlayerIndex == HeroPosition || a.PlayerIndex == playerNum)
                    .ToList();

                if (filtered.Count < 2) return 0;

                var heroIndex = filtered.FindIndex(a => a.PlayerIndex == HeroPosition);
                var oppIndex = filtered.FindIndex(a => a.PlayerIndex == playerNum);
                return heroIndex < oppIndex ? 1 : 0;
            }

            var heroPreflopIndex = noneActions.FindIndex(a => a.PlayerIndex == HeroPosition);
            var oppPreflopIndex = noneActions.FindIndex(a => a.PlayerIndex == playerNum);

            if (heroPreflopIndex != -1 && oppPreflopIndex != -1) return heroPreflopIndex > oppPreflopIndex ? 0 : 1;

            return heroPreflopIndex == -1 ? 0 : 1;
        }

        /// <summary>
        /// Extracts a heads‐up action log from the full multi‐player log.
        /// Only actions from hero (player1) and the chosen opponent (playerNum) are kept.
        /// The parameter pos indicates position of hero:
        ///     pos == 0: hero acts last (dealer)
        ///     pos == 1: hero acts first
        /// If in a phase an expected action is missing (“leak”), a synthetic action is inserted:
        ///  – If the expected first mover is missing, a Check is added.
        ///  – If the expected second mover is missing, then if the first mover’s action was aggressive (Bet, Raise, or AllIn)
        ///    a Fold is inserted; otherwise a Check.
        /// Processing stops as soon as one of the two players folds.
        /// </summary>
        public static List<PlayerAction> ExtractActions(List<PlayerAction> actions, int playerNum, int pos)
        {
            var result = new List<PlayerAction>();

            // Filter for heads-up actions only (hero = 1 and opponent = playerNum)
            var filtered = actions
                .Where(a => a.PlayerIndex == HeroPosition || a.PlayerIndex == playerNum)
                .ToList();

            // We'll process actions grouped by phase.
            var currentPhase = PokerPhase.None;
            List<PlayerAction> phaseActions = [];

            // Process filtered actions by phase.
            foreach (var action in filtered)
            {
                if (action.Phase != currentPhase)
                {
                    FlushPhase();
                    currentPhase = action.Phase;
                }

                if (action.ActionType == PokerActionType.Fold)
                {
                    FlushPhase();
                    result.Add(action);
                    return result;
                }

                phaseActions.Add(action);
            }

            FlushPhase();

            return result;

            void FlushPhase()
            {
                if (currentPhase == PokerPhase.None)
                {
                    result.AddRange(phaseActions.OrderBy(x => x.Amount));
                    phaseActions.Clear();
                    return;
                }

                int expectedFirst, expectedSecond;
                if ((currentPhase == PokerPhase.Preflop && pos == 0) || (currentPhase != PokerPhase.Preflop && pos == 1))
                {
                    expectedFirst = HeroPosition;
                    expectedSecond = playerNum;
                }
                else
                {
                    expectedFirst = playerNum;
                    expectedSecond = HeroPosition;
                }

                List<PlayerAction> processedPhase = [];
                List<PlayerAction> currentRound = [];

                foreach (var act in phaseActions)
                {
                    switch (currentRound.Count)
                    {
                        case 0:
                        {
                            if (act.PlayerIndex != expectedFirst)
                            {
                                currentRound.Add(new PlayerAction(expectedFirst, PokerActionType.Check, 0, currentPhase,
                                    "Synthetic1"));
                            }

                            currentRound.Add(act);
                            break;
                        }
                        case 1 when act.PlayerIndex != expectedSecond:
                            if (currentRound[0].ActionType is not (PokerActionType.Bet or PokerActionType.Raise
                                or PokerActionType.AllIn))
                            {
                                currentRound.Add(new PlayerAction(expectedSecond, PokerActionType.Check, 0,
                                    currentPhase, "Synthetic3"));
                            }

                            processedPhase.AddRange(currentRound);
                            currentRound.Clear();
                            currentRound.Add(act);
                            break;
                        case 1:
                            // Expected second actor is present.
                            currentRound.Add(act);
                            processedPhase.AddRange(currentRound);
                            currentRound.Clear();
                            break;
                        default:
                        {
                            processedPhase.AddRange(currentRound);
                            currentRound.Clear();
                            if (act.PlayerIndex != expectedFirst)
                            {
                                currentRound.Add(new PlayerAction(expectedFirst, PokerActionType.Check, 0, currentPhase,
                                    "Synthetic5"));
                            }

                            currentRound.Add(act);
                            break;
                        }
                    }

                    // If any action is a fold, the phase (and overall processing) stops.
                    if (act.ActionType == PokerActionType.Fold)
                    {
                        break;
                    }
                }

                if (currentRound.Count == 1 && 
                    currentRound[0].ActionType is not (PokerActionType.Call or PokerActionType.Bet 
                        or PokerActionType.Raise or PokerActionType.AllIn))
                {
                    currentRound.Add(new PlayerAction(expectedSecond, PokerActionType.Check, 0, currentPhase, "Synthetic7"));
                }

                processedPhase.AddRange(currentRound);
                result.AddRange(processedPhase);
                phaseActions.Clear();
            }
        }
    }
}