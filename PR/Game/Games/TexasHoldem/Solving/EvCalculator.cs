using System.Collections.Generic;
using System.Linq;
using Game.Games.TexasHoldem.Model;
using PT.Algorithm.Model;

namespace Game.Games.TexasHoldem.Solving;

public record EvResult(
    decimal Pot,
    decimal MyInvestment,
    double Equity, // efektywna equity (np. 0.175 oznacza 17,5%)
    decimal ExpectedWinnings, // oczekiwany zwrot z puli
    decimal NetEv // EV netto (zysk/strata po odliczeniu wkładu)
)
{
    public override string ToString()
    {
        return $"P: {Pot}, MI: {MyInvestment}, " +
               $"EQ: {Equity:P2}, EXP: {ExpectedWinnings}, EV: {NetEv}";
    }
}

public static class EvCalculator
{
    /// <summary>
    /// Oblicza EV dla naszego gracza na podstawie listy akcji, wyniku symulacji Monte Carlo i całkowitej puli.
    /// </summary>
    public static EvResult CalculateEv(List<PlayerAction> gameActions, MonteCarloResult monteCarloResult, decimal pot)
    {
        // Sumujemy nasz wkład – bierzemy pod uwagę akcje, które realnie wpłynęły do puli.
        decimal myInvestment = gameActions
            .Where(a => a.PlayerIndex == 1 &&
                        (a.ActionType == PokerActionType.Put ||
                         a.ActionType == PokerActionType.Call ||
                         a.ActionType == PokerActionType.Bet ||
                         a.ActionType == PokerActionType.Raise ||
                         a.ActionType == PokerActionType.AllIn))
            .Sum(a => a.Amount);

        // Jeśli gracz spasował, EV = -wkład
        if (gameActions.Any(a => a.PlayerIndex == 1 && a.ActionType == PokerActionType.Fold))
        {
            return new EvResult(pot, myInvestment, 0, 0, -myInvestment);
        }

        // Efektywna equity: przy wygranej dostajemy całą pulę, a przy remisie połowę.
        double effectiveEquity = monteCarloResult.Better + monteCarloResult.Exact / 2.0;

        // Oczekiwany zwrot z puli
        decimal expectedWinnings = (decimal) effectiveEquity * pot;

        // EV netto = oczekiwany zwrot - nasz wkład
        decimal netEv = expectedWinnings - myInvestment;

        return new EvResult(pot, myInvestment, effectiveEquity, expectedWinnings, netEv);
    }

    public static PlayerAction MapEvToAction(EvResult evResult, decimal requiredCallAmount, PokerPhase phase)
    {
        // Jeśli EV netto jest ujemne, dalsza gra nie jest opłacalna – fold.
        if (evResult.NetEv < 0)
        {
            return new PlayerAction(1, PokerActionType.Fold, 0, phase);
        }

        // Jeśli wymagana jest wpłata (call), podejmujemy decyzję na podstawie relacji EV netto do requiredCallAmount.
        if (requiredCallAmount > 0)
        {
            // Obliczamy stosunek EV netto do wymaganej kwoty calla.
            decimal ratio = evResult.NetEv / requiredCallAmount;

            if (ratio < 1.0m)
            {
                // EV jest dodatnie, ale nie rekompensuje nawet samej wpłaty – call.
                return new PlayerAction(1, PokerActionType.Call, requiredCallAmount, phase);
            }
            else if (ratio < 2.0m)
            {
                // EV jest umiarkowanie dodatnie – wykonujemy niewielki raise.
                // Przykładowo: raise = call + dodatkowe 50% wartości calla.
                decimal raiseAmount = requiredCallAmount + (requiredCallAmount * 0.5m);
                return new PlayerAction(1, PokerActionType.Raise, raiseAmount, phase);
            }
            else
            {
                // EV jest znacząco dodatnie – agresywny raise.
                // Podwyższamy stawkę proporcjonalnie do nadwyżki EV ponad call.
                decimal extra = requiredCallAmount * (ratio - 1.0m);
                decimal raiseAmount = requiredCallAmount + extra;
                return new PlayerAction(1, PokerActionType.Raise, raiseAmount, phase);
            }
        }
        else
        {
            // Jeśli nie musimy nic dodatkowo wpłacać (np. mamy możliwość check),
            // rozważamy, czy warto postawić (bet) w zależności od EV netto w stosunku do puli.
            // Używamy relacji EV netto do całkowitej puli.
            decimal ratio = evResult.NetEv / evResult.Pot;
            if (ratio > 0.05m) // Jeśli EV netto przekracza np. 5% puli, rozważamy bet.
            {
                // Kwota betu skalowana jest według przewagi – tu przykładowo bet = pula * ratio.
                decimal betAmount = evResult.Pot * ratio;
                return new PlayerAction(1, PokerActionType.Bet, betAmount, phase);
            }
            else
            {
                return new PlayerAction(1, PokerActionType.Check, 0, phase);
            }
        }
    }
}