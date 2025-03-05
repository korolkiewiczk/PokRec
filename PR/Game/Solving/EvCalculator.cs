using System.Collections.Generic;
using System.Linq;
using Game.Common.Model;
using PT.Algorithm.Model;

namespace Game.Solving;

public static class EvCalculator
{
    public static EvResult CalculateEv(List<PlayerAction> gameActions, MonteCarloResult monteCarloResult, decimal pot)
    {
        decimal myInvestment = gameActions
            .Where(a => a.PlayerIndex == 1 &&
                        (a.ActionType == PokerActionType.Put ||
                         a.ActionType == PokerActionType.Call ||
                         a.ActionType == PokerActionType.Bet ||
                         a.ActionType == PokerActionType.Raise ||
                         a.ActionType == PokerActionType.AllIn))
            .Sum(a => a.Amount);

        if (gameActions.Any(a => a.PlayerIndex == 1 && a.ActionType == PokerActionType.Fold))
        {
            return new EvResult(pot, myInvestment, 0, 0, -myInvestment);
        }

        double effectiveEquity = monteCarloResult.Better + monteCarloResult.Exact / 2.0;

        decimal expectedWinnings = (decimal) effectiveEquity * pot;

        decimal netEv = expectedWinnings - myInvestment;

        return new EvResult(pot, myInvestment, effectiveEquity, expectedWinnings, netEv);
    }
}