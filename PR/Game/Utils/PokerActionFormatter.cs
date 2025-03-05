using System;
using System.Collections.Generic;
using Game.Common.Model;

namespace Game.Utils
{
    public static class PokerActionFormatter
    {
        public static string FormatActions(List<PlayerAction> actions, decimal smallBlind)
        {
            var actionStrings = new List<string>();
            decimal lastAmount = 0;

            foreach (var action in actions)
            {
                switch (action.ActionType)
                {
                    case PokerActionType.Fold:
                        actionStrings.Add("F");
                        lastAmount = 0;
                        break;

                    case PokerActionType.Check:
                    case PokerActionType.Call:
                        actionStrings.Add("C");
                        lastAmount = 0;
                        break;

                    case PokerActionType.Put:
                    case PokerActionType.Bet:
                    case PokerActionType.Raise:
                        int relativeBet = (int) Math.Round((action.Amount - lastAmount) / smallBlind);
                        if (relativeBet >= 0)
                        {
                            actionStrings.Add($"R{relativeBet}");
                            lastAmount = action.Amount;
                        }
                        else
                        {
                            actionStrings.Add($"R0");
                        }

                        break;

                    case PokerActionType.AllIn:
                        int relativeAllIn = (int) Math.Round((action.Amount - lastAmount) / smallBlind);
                        actionStrings.Add($"A{relativeAllIn}");
                        lastAmount = action.Amount;
                        break;
                }
            }

            return string.Join(",", actionStrings);
        }
    }
}