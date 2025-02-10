using System.Collections.Generic;
using System.Linq;
using Game.Games.TexasHoldem.Model;

namespace Game.Games.TexasHoldem.Solving;

public record PlayerStats(
    int Hands = 0,
    int VPIP = 0,
    int PFR = 0,
    int ThreeBet = 0,
    int FoldToThreeBet = 0,
    int CBetFlop = 0,
    int FoldToCBetFlop = 0,
    int WTSD = 0
);

public record PlayerStatsRelative(
    int Hands,
    double VPIP,
    double PFR,
    double ThreeBet,
    double FoldToThreeBet,
    double CBetFlop,
    double FoldToCBetFlop,
    double WTSD
)
{
    public override string ToString()
    {
        return $"HANDS: {Hands}, VPIP: {VPIP:F1}%, PFR: {PFR:F1}%, 3-Bet: {ThreeBet:F1}%, " +
               $"Fold to 3-Bet: {FoldToThreeBet:F1}%, C-Bet Flop: {CBetFlop:F1}%, " +
               $"Fold to C-Bet: {FoldToCBetFlop:F1}%, WTSD: {WTSD:F1}%";
    }
}

public static class PlayerStatsExtensions
{
    public static PlayerStatsRelative ToRelativeStats(this PlayerStats stats)
    {
        if (stats.Hands == 0)
        {
            return new PlayerStatsRelative(0,0, 0, 0, 0, 0, 0, 0);
        }

        return new PlayerStatsRelative(
            stats.Hands,
            VPIP: (double) stats.VPIP / stats.Hands * 100,
            PFR: (double) stats.PFR / stats.Hands * 100,
            ThreeBet: (double) stats.ThreeBet / stats.Hands * 100,
            FoldToThreeBet: (double) stats.FoldToThreeBet / stats.Hands * 100,
            CBetFlop: (double) stats.CBetFlop / stats.Hands * 100,
            FoldToCBetFlop: (double) stats.FoldToCBetFlop / stats.Hands * 100,
            WTSD: (double) stats.WTSD / stats.Hands * 100
        );
    }

    public static string ToDebugString(this Dictionary<string, PlayerStats> playersStats)
    {
        return string.Join("\n", playersStats.Select(x =>
            $"{x.Key} = {string.Join("|", ToRelativeStats(x.Value))}"));
    }
}

public static class PlayerStatistics
{
    public static PlayerStats AddToStats(PlayerStats currentStats, int playerIndex, List<PlayerAction> actions)
    {
        var preflopActions = actions.Where(a => a.Phase == PokerPhase.Preflop).ToList();
        var flopActions = actions.Where(a => a.Phase == PokerPhase.Flop).ToList();
        int hands = 1;

        // 1. VPIP – gracz wkłada pieniądze do puli preflop (Call, Bet, Raise, AllIn)
        int vpip = preflopActions.Any(a => a.PlayerIndex == playerIndex &&
                                           (a.ActionType == PokerActionType.Call ||
                                            a.ActionType == PokerActionType.Bet ||
                                            a.ActionType == PokerActionType.Raise ||
                                            a.ActionType == PokerActionType.AllIn))
            ? 1
            : 0;

        // 2. PFR – gracz agresywnie podbija preflop (Raise lub AllIn)
        int pfr = preflopActions.Any(a => a.PlayerIndex == playerIndex &&
                                          (a.ActionType == PokerActionType.Raise ||
                                           a.ActionType == PokerActionType.AllIn))
            ? 1
            : 0;

        // 3. 3-Bet – sprawdzamy, czy pierwsza agresywna akcja gracza (Raise lub AllIn) nastąpiła po agresji przeciwnika
        int threeBet = 0;
        var playerPreflopAggressive = preflopActions
            .Where(a => a.PlayerIndex == playerIndex &&
                        (a.ActionType == PokerActionType.Raise || a.ActionType == PokerActionType.AllIn))
            .OrderBy(a => preflopActions.IndexOf(a))
            .FirstOrDefault();

        if (playerPreflopAggressive != null)
        {
            int actionIndex = preflopActions.IndexOf(playerPreflopAggressive);
            bool opponentRaisedBefore = preflopActions
                .Take(actionIndex)
                .Any(a => a.PlayerIndex != playerIndex &&
                          (a.ActionType == PokerActionType.Raise || a.ActionType == PokerActionType.AllIn));
            if (opponentRaisedBefore)
            {
                threeBet = 1;
            }
        }

        // 4. Fold to 3-Bet – uproszczony model:
        // Jeśli gracz wykonał Call, a następnie (po agresji przeciwnika) spasował, uznajemy to za fold na 3-bet.
        int foldTo3Bet = 0;
        bool hasCalled = false;
        bool opponentAggressiveAfterCall = false;
        foreach (var action in preflopActions)
        {
            if (action.PlayerIndex == playerIndex)
            {
                if (action.ActionType == PokerActionType.Call)
                {
                    hasCalled = true;
                }
                else if (hasCalled && opponentAggressiveAfterCall && action.ActionType == PokerActionType.Fold)
                {
                    foldTo3Bet = 1;
                    break;
                }
            }
            else // akcje przeciwników
            {
                if (hasCalled && (action.ActionType == PokerActionType.Raise ||
                                  action.ActionType == PokerActionType.AllIn))
                {
                    opponentAggressiveAfterCall = true;
                }
            }
        }

        // 5. C-Bet Flop – jeśli gracz agresywnie zagrał preflop (PFR) i jako pierwszy na flopie betował (Bet, Raise, AllIn)
        int cbetFlop = 0;
        if (pfr == 1)
        {
            var playerFlopAction = flopActions.FirstOrDefault(a => a.PlayerIndex == playerIndex);
            if (playerFlopAction != null &&
                (playerFlopAction.ActionType == PokerActionType.Bet ||
                 playerFlopAction.ActionType == PokerActionType.Raise ||
                 playerFlopAction.ActionType == PokerActionType.AllIn))
            {
                cbetFlop = 1;
            }
        }

        // 6. Fold to C-Bet Flop – jeśli gracz spasował na flopie po tym, gdy przeciwnik wykonał zakład (Bet, Raise, AllIn)
        int foldToCbetFlop = 0;
        bool opponentBetSeen = false;
        foreach (var action in flopActions)
        {
            if (action.PlayerIndex != playerIndex)
            {
                if (action.ActionType == PokerActionType.Bet ||
                    action.ActionType == PokerActionType.Raise ||
                    action.ActionType == PokerActionType.AllIn)
                {
                    opponentBetSeen = true;
                }
            }
            else // akcje gracza
            {
                if (opponentBetSeen && action.ActionType == PokerActionType.Fold)
                {
                    foldToCbetFlop = 1;
                    break;
                }
            }
        }

        // 7. WTSD (Went To Showdown) – zakładamy, że gracz poszedł do showdown, jeśli nie spasował w żadnej fazie
        int wtsd = actions.Any(a => a.PlayerIndex == playerIndex && a.ActionType == PokerActionType.Fold) ? 0 : 1;

        return new PlayerStats(
            Hands: currentStats.Hands + hands,
            VPIP: currentStats.VPIP + vpip,
            PFR: currentStats.PFR + pfr,
            ThreeBet: currentStats.ThreeBet + threeBet,
            FoldToThreeBet: currentStats.FoldToThreeBet + foldTo3Bet,
            CBetFlop: currentStats.CBetFlop + cbetFlop,
            FoldToCBetFlop: currentStats.FoldToCBetFlop + foldToCbetFlop,
            WTSD: currentStats.WTSD + wtsd
        );
    }
}

/*
Najważniejsze statystyki (kluczowe do oceny stylu gry przeciwnika)
VPIP (Voluntarily Put Money In Pot) – % rąk, w których gracz dobrowolnie wkłada żetony do puli (czyli nie jest to BB lub SB). Wskazuje, czy jest tight (mało rąk) czy loose (dużo rąk).
PFR (Preflop Raise) – % rąk, w których gracz podbija preflop. W połączeniu z VPIP pozwala określić styl gracza (np. loose-aggressive, tight-passive).
3-Bet – % sytuacji, w których gracz re-raisuje preflop. Ważne dla oceny agresji.
Fold to 3-Bet – Jak często gracz pasuje na re-raise preflop? Jeśli często, można go exploitiwać.
C-Bet Flop – Jak często gracz kontynuuje agresję po podbiciu preflop? Wysoki % oznacza, że często blefuje.
Fold to C-Bet Flop – Jak często gracz pasuje na continuation bet? Jeśli często, można przeciwko niemu częściej c-betować.
WTSD (Went to Showdown) – Jak często gracz idzie do showdownu? Wysoki % oznacza, że jest calling station.*/