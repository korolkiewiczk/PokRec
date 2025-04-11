using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Game.Common.Model;
using Game.Utils;
using PT.Algorithm.Model;
using PT.Poker.Model;
using PT.Poker.Resolving;

namespace Game.Solving;

public static class PokerHelper
{
    public static List<int> FilterActiveIndices(IEnumerable<decimal?> stack)
    {
        return stack
            .Select((s, index) => new {StackValue = s, Index = index})
            .Where(x => x.StackValue.HasValue)
            .Select(x => x.Index)
            .ToList();
    }

    public static List<decimal?> RemapPreviousStacks(
        List<decimal?> stack,
        List<int> activeIndices,
        List<int> prevActiveIndices,
        List<decimal?> prevStacks)
    {
        if (prevStacks == null)
        {
            return stack.ToList();
        }

        if (stack.Count < prevStacks.Count)
        {
            return activeIndices.Select(i => i < prevStacks.Count ? prevStacks[i] : 0).ToList();
        }

        if (stack.Count > prevStacks.Count && prevActiveIndices != null)
        {
            return activeIndices
                .Select(activeIndex => prevActiveIndices.IndexOf(activeIndex))
                .Select((prevIndex, i) => prevIndex != -1 ? prevStacks[prevIndex] : stack[i]).ToList();
        }

        return prevStacks;
    }

    public static (MonteCarloResult? monteCarloResult, PokerLayouts? bestLayout) SolvePlayerLayout(
        IList<Card> playerCards,
        Place opponents, IList<Card> flopCards, IList<Card> turnCards, IList<Card> riverCards, List<PlayerStatsRelative> stats, 
        int monteCarloIterations = 250)
    {
        MonteCarloResult? monteCarloResult = null;
        PokerLayouts? bestLayout = null;
        if (playerCards.Count != 0)
        {
            int countPlayers = opponents.Count + 1; // +1 for the player
            monteCarloResult = ComputeEquityWithStats(playerCards,
                flopCards.Union(turnCards).Union(riverCards).ToList(),
                countPlayers, stats, monteCarloIterations);
            var allCards = playerCards.Union(flopCards).Union(turnCards).Union(riverCards).ToArray();
            var layoutResolver = new LayoutResolver(new CardLayout(allCards));
            bestLayout = layoutResolver.PokerLayout;
        }

        return (monteCarloResult, bestLayout);
    }

    private static MonteCarloResult ComputeEquityWithStats(IEnumerable<Card> myCards, IEnumerable<Card> boardCards,
        int numOfPlayers, List<PlayerStatsRelative> stats, int monteCarloIterations = 250)
    {
        RandomSetDefinition arg = new RandomSetDefinition
        {
            MyLayout = new CardLayout(myCards.ToArray()),
            NumOfPlayers = numOfPlayers,
            Board = boardCards.ToArray()
        };

        var result = EquityCalculator.CalculateTwoStageEquityWithStats(arg, monteCarloIterations, stats);
        return result;
    }

    public static GameBets InferActions(
        PokerPhase phase,
        IList<decimal?> previousStacks,
        IList<decimal?> currentStacks,
        IList<bool> opponentsInGame,
        IList<bool> previousOpponentsInGame,
        int dealerPosition,
        int numPlayers,
        StartingBets startingBets,
        IList<decimal> currentStreetContributions,
        ref decimal currentStreetHighestBet,
        PokerActionType[] lastActionThisStreet)
    {
        if (previousStacks.Count == 0) return new GameBets([], new StartingBets(0, 0, 0));
        var actions = new List<PlayerAction>();

        // Calculate stack differences
        var contributions = new decimal[previousStacks.Count];
        for (int i = 0; i < previousStacks.Count; i++)
        {
            if (previousStacks[i].HasValue && currentStacks[i].HasValue)
                contributions[i] = previousStacks[i].Value - currentStacks[i].Value;
        }

        // First pass - identify ante amount from players not in game and blind amounts

        if (startingBets == null)
        {
            var (ante, smallBlind, bigBlind) = DeduceBlindsBasingOnPosition(dealerPosition, contributions);

            if (smallBlind < 0 || bigBlind < 0)
            {
                (ante, smallBlind, bigBlind) = DeduceBlindsBasingOnContributions(contributions);
                startingBets = new StartingBets(ante, smallBlind, bigBlind);
                actions.AddRange(contributions.Select((x, i) =>
                    new PlayerAction(i + 1, PokerActionType.Put, x, PokerPhase.None)));
            }
            else
            {
                startingBets = new StartingBets(ante, smallBlind, bigBlind);

                actions.Add(new PlayerAction(dealerPosition % numPlayers + 1, PokerActionType.Put,
                    smallBlind + ante,
                    phase));
                actions.Add(new PlayerAction((dealerPosition + 1) % numPlayers + 1, PokerActionType.Put,
                    bigBlind + ante,
                    phase));
                for (int i = 0; i < numPlayers - 2; i++)
                {
                    actions.Add(new PlayerAction((dealerPosition + 2 + i) % numPlayers + 1,
                        PokerActionType.Put, ante, phase));
                }
            }
        }

        if (phase == PokerPhase.Preflop && currentStreetHighestBet < startingBets.BigBlind)
        {
            currentStreetHighestBet = startingBets.BigBlind;
        }

        // Process actions for active players
        if (phase != PokerPhase.None)
        {
            for (int k = 0; k < numPlayers; k++)
            {
                var i = (dealerPosition + k) % numPlayers;
                bool wasInGame = (i == 0) || (previousOpponentsInGame?[i - 1] ?? false);
                bool isInGame = (i == 0) || opponentsInGame[i - 1];

                // 1) FOLD: if seat was in but now is out
                if (wasInGame && !isInGame)
                {
                    actions.Add(new PlayerAction(i + 1, PokerActionType.Fold, 0, phase));
                    lastActionThisStreet[i] = PokerActionType.Fold;
                    continue;
                }

                // 2) STILL IN
                if (wasInGame)
                {
                    decimal amountPutIn = contributions[i];
                    bool isAllIn = currentStacks[i] == 0;

                    if (amountPutIn > 0)
                    {
                        // Existing logic: Bet/Call/Raise/AllIn
                        currentStreetContributions[i] += amountPutIn;

                        var actionType = DetermineActionType(
                            currentStreetHighestBet,
                            currentStreetContributions[i],
                            isAllIn
                        );

                        actions.Add(new PlayerAction(i + 1, actionType, amountPutIn, phase));
                        lastActionThisStreet[i] = actionType;

                        // Possibly update the highest bet
                        if (currentStreetContributions[i] > currentStreetHighestBet)
                            currentStreetHighestBet = currentStreetContributions[i];
                    }
                    else
                    {
                        bool alreadyChecked = lastActionThisStreet[i] != PokerActionType.None;

                        bool noOutstandingBet = currentStreetContributions[i] >= currentStreetHighestBet;

                        if (!alreadyChecked && noOutstandingBet)
                        {
                            actions.Add(new PlayerAction(i + 1, PokerActionType.Check, 0, phase));
                            lastActionThisStreet[i] = PokerActionType.Check;
                        }
                    }
                }
            }
        }

        return new GameBets(actions.ToImmutableList(), startingBets);
    }

    private static PokerActionType DetermineActionType(
        decimal currentStreetHighestBet,
        decimal playerContributionInThisStreet,
        bool isAllIn
    )
    {
        if (isAllIn)
            return PokerActionType.AllIn;

        if (currentStreetHighestBet == 0)
        {
            return PokerActionType.Bet;
        }

        if (playerContributionInThisStreet <= currentStreetHighestBet)
        {
            return PokerActionType.Call;
        }

        if (playerContributionInThisStreet > currentStreetHighestBet)
        {
            return PokerActionType.Raise;
        }

        throw new InvalidOperationException("Unable to determine action type.");
    }

    public static Place RemapPlace(Place p, IList<int> active, int originalNumPlayers)
    {
        var remappedPlace = new Place();
        var origPlaceBooleans = p.Places(originalNumPlayers);

        for (int i = 0; i < origPlaceBooleans.Count; i++)
        {
            if (origPlaceBooleans[i] && active.Contains(i))
            {
                var newIndex = active.IndexOf(i);
                remappedPlace.Add(newIndex);
            }
        }

        return remappedPlace;
    }

    private static (decimal ante, decimal smallBlind, decimal bigBlind) DeduceBlindsBasingOnPosition(
        int dealerPosition,
        decimal[] contributions)
    {
        int sbPos = (dealerPosition + 1) % contributions.Length;
        int bbPos = (dealerPosition + 2) % contributions.Length;
        decimal ante = 0;
        decimal smallBlind = 0;
        decimal bigBlind = 0;
        for (int i = 0; i < contributions.Length; i++)
        {
            var playerPos = (i + 1) % contributions.Length;
            var isSmallBlind = playerPos == sbPos;
            var isBigBlind = playerPos == bbPos;
            var contribution = contributions[i];
            if (!isSmallBlind && !isBigBlind && ante == 0 && contribution > 0)
            {
                ante = contribution;
            }

            if (isSmallBlind)
            {
                smallBlind = contribution;
            }

            if (isBigBlind)
            {
                bigBlind = contribution;
            }
        }

        smallBlind -= ante;
        bigBlind -= ante;

        if (smallBlind <= 0)
        {
            smallBlind = bigBlind / 2;
        }

        if (bigBlind <= 0)
        {
            bigBlind = smallBlind * 2;
        }

        return (ante, smallBlind, bigBlind);
    }

    private static (decimal Ante, decimal SmallBlind, decimal BigBlind) DeduceBlindsBasingOnContributions(
        decimal[] contributions)
    {
        var validContributions = (contributions.Any(x => x > 0)
            ? contributions.Where(x => x > 0)
            : contributions).ToArray();

        var groups = validContributions.GroupBy(x => x).ToList();
        decimal ante = groups
            .Where(g => g.Key > 0 && g.Count() > 1)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => g.Key)
            .FirstOrDefault();

        var extras = validContributions.Distinct()
            .Where(x => x > ante)
            .Select(x => x - ante)
            .OrderBy(x => x)
            .ToList();

        var (smallBlind, bigBlind) = extras.Count switch
        {
            0 => (0m, 0m),
            _ => (extras.First(), 2 * extras.First())
        };

        return (ante, smallBlind, bigBlind);
    }

    public static PokerPhase DeterminePokerPhase(List<Card> flopCards, List<Card> turnCards, List<Card> riverCards)
    {
        if (riverCards.Count != 0)
            return PokerPhase.River;
        if (turnCards.Count != 0)
            return PokerPhase.Turn;
        if (flopCards.Count != 0)
            return PokerPhase.Flop;
        return PokerPhase.Preflop;
    }
}