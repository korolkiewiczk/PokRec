using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common;
using Game.Common.Model;
using Game.Common.MultiRegionMatchers;
using Game.Common.RegionMatchers;
using Game.Common.Utils;
using Game.Utils;
using Game.Datalayer;

namespace Game.Solving
{
    public class Poker
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType!);

        private Flop _flop;
        private Turn _turn;
        private River _river;
        private PlayerCards _playerCards;
        private Position _position;
        private Opponent _opponent;
        private Stack _stack;
        private Nickname _nickname;
        private int _numPlayers;
        private IDictionary<string, ReconResult> _state;
        private Decision _decision;
        private Pot _pot;
        private MatchResults _lastMatchResults;

        private PokerResults _prevPokerResults;
        private PokerPhase _previousPhase;
        private StartingBets _startingBets;
        private readonly List<PlayerAction> _gameActions = new();
        private Place _lastPosition;
        private decimal[] _currentStreetContributions;
        private decimal _currentStreetHighestBet;
        private IList<bool> _previousOpponentsInGame;
        private PokerActionType[] _lastActionThisStreet;
        private List<int> _activeIndices;
        private List<int> _prevActiveIndices;

        private Dictionary<int, List<string>> _nickNamesAtPos = new();
        private readonly Dictionary<string, PlayerStats> _playerStats = new();

        public Poker(Board board)
        {
            Board = board;
            InitializeMatchers();
            InitializePlayerStatsFromDatabase();
        }

        public void SetState(IDictionary<string, ReconResult> state)
        {
            _state = state;
        }

        public Board Board { get; }

        public StartingBets StartingBets => _startingBets;

        public List<PlayerAction> GameActions => _gameActions;

        public PokerDebugFlags DebugFlags { get; set; } = PokerDebugFlags.None;

        public Dictionary<string, PlayerStats> PlayerStats => _playerStats;

        public List<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> regionSpecs =
            [
                _flop.GetRegionSpec(),
                _turn.GetRegionSpec(),
                _river.GetRegionSpec(),
                _playerCards.GetRegionSpec(),
                _decision.GetRegionSpec(),
            ];
            regionSpecs.AddRange(_position.GetRegionSpecs());
            regionSpecs.AddRange(_opponent.GetRegionSpecs());
            regionSpecs.AddRange(_stack.GetRegionSpecs());
            regionSpecs.AddRange(_nickname.GetRegionSpecs());
            regionSpecs.AddRange(_pot.GetRegionSpecs());
            return regionSpecs;
        }

        public PokerResults Solve()
        {
            var reconResults = GetReconResults();
            var playerCards = _playerCards.Match(reconResults.PlayerResult);
            var flopCards = _flop.Match(reconResults.FlopResult);
            var turnCards = _turn.Match(reconResults.TurnResult);
            var riverCards = _river.Match(reconResults.RiverResult);
            Place position = _position.Match(reconResults.PositionResults);
            Place opponents = _opponent.Match(reconResults.OpponentResults);
            List<decimal?> stack = _stack.Match(reconResults.StackResults);
            List<string> nicknames = _nickname.Match(reconResults.NicknameResults);
            var isDecision = _decision.Match(reconResults.DecisionResult);
            var pot = _pot.Match([reconResults.PotResult]);

            var phase = PokerHelper.DeterminePokerPhase(flopCards, turnCards, riverCards);
            if (_previousPhase == PokerPhase.None)
            {
                _previousPhase = phase;
            }

            // Check if position has changed - new game then
            if (_lastPosition != null && position != _lastPosition)
            {
                ClearPropertiesOnNewStreet();
            }

            _lastPosition = position;
            _activeIndices ??= PokerHelper.FilterActiveIndices(stack);
            var numPlayers = _activeIndices.Count;
            stack = _activeIndices.Select(i => stack[i]).ToList();

            if (_prevPokerResults != null)
            {
                var remappedStacks = PokerHelper.RemapPreviousStacks(stack, _activeIndices, _prevActiveIndices,
                    _prevPokerResults.MatchResults.Stacks);
                _prevPokerResults = _prevPokerResults with
                {
                    MatchResults = _prevPokerResults.MatchResults with {Stacks = remappedStacks}
                };
            }

            _prevActiveIndices = _activeIndices;

            nicknames = _activeIndices.Select(i => nicknames[i]).ToList();

            for (var i = 0; i < nicknames.Count; i++)
            {
                if (!_nickNamesAtPos.ContainsKey(i))
                    _nickNamesAtPos[i] = new List<string>();
                _nickNamesAtPos[i].Add(nicknames[i]);
            }

            position = PokerHelper.RemapPlace(position, _activeIndices, _numPlayers);
            opponents = PokerHelper.RemapPlace(opponents, _activeIndices.Where(x => x != 0).Select(x => x - 1).ToList(),
                _numPlayers - 1);

            PokerPosition? pokerPosition = position.GetPokerPosition(numPlayers);

            var matchResults =
                new MatchResults(playerCards, flopCards, turnCards, riverCards,
                    position, opponents, nicknames, stack, isDecision, pot);

            var opponentsInGame = opponents.Places(numPlayers - 1);
            
            var statsRelative = GetPlayerStatsRelative(opponentsInGame);

            var (monteCarloResult, bestLayout) = PokerHelper.SolvePlayerLayout(playerCards, opponents, flopCards, turnCards, riverCards, statsRelative, 250);
            var playerFold =
                _gameActions.FirstOrDefault(x => x.PlayerIndex == 1 && x.ActionType == PokerActionType.Fold);
            if (bestLayout == null && playerFold == null)
            {
                _gameActions.Add(new PlayerAction(1, PokerActionType.Fold, 0, _previousPhase));
            }
            else if (bestLayout != null && playerFold != null)
            {
                _gameActions.Remove(playerFold);
            }

            var pokerResult = new PokerResults(
                reconResults,
                matchResults,
                monteCarloResult,
                bestLayout,
                pokerPosition,
                opponentsInGame.ToImmutableList(),
                phase,
                true, null!);

            // Initialize snapshots if they're not set
            _prevPokerResults ??= pokerResult;
            _previousOpponentsInGame ??= opponentsInGame.ToList();
            _lastActionThisStreet ??= new PokerActionType[numPlayers];

            if (_lastMatchResults == null || !_lastMatchResults.Equals(matchResults))
            {
                DebuggingLogs(matchResults, pokerPosition, opponentsInGame);

                _lastMatchResults = matchResults;

                // Always infer actions since the start of the phase
                if (_prevPokerResults != pokerResult)
                {
                    var gameBets = PokerHelper.InferActions(
                        _previousPhase,
                        _prevPokerResults.MatchResults.Stacks,
                        matchResults.Stacks,
                        opponentsInGame,
                        _previousOpponentsInGame,
                        matchResults.Position.Pos,
                        numPlayers,
                        _startingBets,
                        _currentStreetContributions,
                        ref _currentStreetHighestBet,
                        _lastActionThisStreet);

                    gameBets = FillWithId(gameBets);

                    var gameBetsActions = gameBets.Actions;

                    DebuggingLogs(gameBets);

                    RemoveRedundantChecks(gameBetsActions, phase);

                    _gameActions.AddRange(gameBetsActions);
                    _startingBets = gameBets.StartingBets;

                    DebuggingLogs(gameBetsActions);
                }
            }
            
            if (isDecision && monteCarloResult.HasValue && pot.HasValue)
            {
                var evResult = EvCalculator.CalculateEv(_gameActions, monteCarloResult.Value, pot.Value);
                DebuggingLogs(evResult);

                pokerResult = pokerResult with {EvResult = evResult};
            }

            var isCorrectPot = IsCorrectPotWithFixActions(matchResults);

            pokerResult = pokerResult with {IsCorrectPot = isCorrectPot};

            _prevPokerResults = pokerResult;
            if (phase != _previousPhase)
            {
                ClearPropertiesOnNewPhase(numPlayers);
            }

            _previousPhase = phase;
            _previousOpponentsInGame = opponentsInGame.ToList();

            return pokerResult;
        }
        
        private void InitializeMatchers()
        {
            var settings = new PokerBoardSettingsParser(Board);
            _numPlayers = settings.Players;

            _flop = new Flop(Board);
            _turn = new Turn(Board);
            _river = new River(Board);
            _playerCards = new PlayerCards(Board);
            _position = new Position(Board, _numPlayers);
            _opponent = new Opponent(Board, _numPlayers - 1);
            _stack = new Stack(_numPlayers);
            _nickname = new Nickname(_numPlayers);
            _decision = new Decision(Board);
            _pot = new Pot();
            _currentStreetContributions = new decimal[_numPlayers];
        }

        private ReconResults GetReconResults()
        {
            var playerResult = GetResult(nameof(PlayerCards));
            var flopResult = GetResult(nameof(Flop));
            var turnResult = GetResult(nameof(Turn));
            var riverResult = GetResult(nameof(River));
            var positionResults = GetResultsPrefixed(nameof(Position)).ToList();
            var opponentResults = GetResultsPrefixed(nameof(Opponent)).ToList();
            var stackResults = GetResultsPrefixed(nameof(Stack)).ToList();
            var nicknameResults = GetResultsPrefixed(nameof(Nickname)).ToList();
            var decisionResult = GetResult(nameof(Decision));
            var potResult = GetResult(nameof(Pot));

            return new ReconResults(playerResult, flopResult, turnResult, riverResult, positionResults,
                opponentResults, stackResults, nicknameResults, decisionResult, potResult);
        }

        private void ClearPropertiesOnNewPhase(int numPlayers)
        {
            Array.Clear(_currentStreetContributions);
            _currentStreetHighestBet = 0;
            _lastActionThisStreet = new PokerActionType[numPlayers];
        }

        private GameBets FillWithId(GameBets gameBets)
        {
            if (_state.TryGetValue("_id", out var value))
            {
                gameBets = gameBets with
                {
                    Actions = gameBets.Actions.Select(x => x with {Id = value.Result})
                        .ToImmutableList()
                };
            }

            return gameBets;
        }

        private void RemoveRedundantChecks(IEnumerable<PlayerAction> gameBetsActions, PokerPhase phase)
        {
            foreach (var gameBetsAction in gameBetsActions)
            {
                if (gameBetsAction.ActionType != PokerActionType.Check)
                {
                    for (var i = _gameActions.Count - 1; i >= 0; i--)
                    {
                        var a = _gameActions[i];
                        if (a.PlayerIndex == gameBetsAction.PlayerIndex &&
                            a.Phase == phase &&
                            a.ActionType == PokerActionType.Check)
                        {
                            _gameActions.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private void ClearPropertiesOnNewStreet()
        {
            UpdateStatistics();

            _gameActions.Clear();
            _previousPhase = PokerPhase.None;
            _startingBets = null;
            _activeIndices = null;
            _previousOpponentsInGame = null;
            _nickNamesAtPos = new Dictionary<int, List<string>>();
        }

        private void UpdateStatistics()
        {
            if (_gameActions.Count == 0) return;
            if (_gameActions[^1].PlayerIndex == 1 && _gameActions[^1].ActionType==PokerActionType.Fold) 
                _gameActions.Remove(_gameActions[^1]);
            var uniquePlayers = _gameActions.Select(x => x.PlayerIndex).Distinct();
            foreach (var playerIndex in uniquePlayers)
            {
                var i = playerIndex - 1;
                var normalizedNickName = NormalizedInputSelector.GetNormalizedFromInputs(_nickNamesAtPos[i]);

                var similarKey = FuzzyKeyFinder.FindSimilarKey(_playerStats.Keys, normalizedNickName, maxDistance: 2);

                if (similarKey != null)
                {
                    normalizedNickName = similarKey;
                }
                else if (!_playerStats.ContainsKey(normalizedNickName))
                {
                    _playerStats[normalizedNickName] = new PlayerStats();
                }

                _playerStats[normalizedNickName] = PlayerStatistics.AddToStats(_playerStats[normalizedNickName],
                    playerIndex, _gameActions);
            }

            DebuggingLogsForPlayerStats();
            SavePlayerStatsToDatabase();
        }
        
        /// <summary>
        /// Saves the current player statistics to the database
        /// </summary>
        private void SavePlayerStatsToDatabase()
        {
            try
            {
                // Skip if there are no player stats to save
                if (_playerStats.Count == 0)
                    return;
                    
                // Create a new repository instance
                var repository = new GameRepository();
                
                // Update player stats in the database
                repository.UpdateStats(_playerStats);
            }
            catch (Exception ex)
            {
                Log.Error("Error saving player statistics to database", ex);
            }
        }

        private bool IsCorrectPotWithFixActions(MatchResults matchResults)
        {
            decimal totalContributions = _gameActions.Sum(action => action.Amount);

            bool isCorrectPot = true;
            if (matchResults.Pot != null)
            {
                isCorrectPot = Math.Abs(totalContributions - matchResults.Pot.Value) < 0.01m;
            }

            if (!isCorrectPot)
            {
                var potDiff = matchResults.Pot - _prevPokerResults.MatchResults.Pot;
                if (potDiff.HasValue)
                {
                    var newGameActionsAmounts = _gameActions.Select(x => x.Amount).ToList();
                    if (newGameActionsAmounts.Count != 0)
                    {
                        var prevInvalidAmount = newGameActionsAmounts[^1];
                        newGameActionsAmounts[^1] = potDiff.Value;
                        var totalContributions2 = newGameActionsAmounts.Sum(x => x);
                        if (matchResults.Pot != null)
                        {
                            var isCorrectPot2 = Math.Abs(totalContributions2 - matchResults.Pot.Value) < 0.01m;
                            if (isCorrectPot2)
                            {
                                _gameActions[^1] = new PlayerAction(Amount: potDiff.Value, Id: _gameActions[^1].Id,
                                    Phase: _gameActions[^1].Phase,
                                    ActionType: potDiff == 0 ? PokerActionType.Check : _gameActions[^1].ActionType,
                                    PlayerIndex: _gameActions[^1].PlayerIndex);
                                if (Math.Abs(_currentStreetHighestBet - prevInvalidAmount) < 0.01m)
                                {
                                    _currentStreetHighestBet = potDiff.Value;
                                    _currentStreetContributions[_gameActions[^1].PlayerIndex - 1] -=
                                        prevInvalidAmount - potDiff.Value;
                                }
                            }
                        }
                    }
                }
            }

            return isCorrectPot;
        }

        private List<ReconResult> GetResultsPrefixed(string name)
        {
            return _state.Where(x => x.Key.StartsWith(name)).OrderBy(x => x.Key)
                .Select(x => x.Value).ToList();
        }

        private ReconResult GetResult(string name)
        {
            _state.TryGetValue(name, out var result);
            return result;
        }
        
        private void InitializePlayerStatsFromDatabase()
        {
            try
            {
                var repository = new GameRepository();
                
                var dbPlayerStats = repository.GetAllStats();
                
                if (dbPlayerStats is {Count: > 0})
                {
                    foreach (var kvp in dbPlayerStats)
                    {
                        _playerStats[kvp.Nickname.Name] = kvp.ToStatsRelative().ToStats();
                    }
                    
                    Log.Info($"Loaded {_playerStats.Count} player statistics from database");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error loading player statistics from database", ex);
            }
        }

        private List<PlayerStatsRelative> GetPlayerStatsRelative(List<bool> opponentsInGame)
        {
            var statsRelative = new List<PlayerStatsRelative>();
            
            // Add stats for each opponent still in the game
            for (var i = 0; i < opponentsInGame.Count; i++)
            {
                if (!opponentsInGame[i]) continue;
                var normalizedNickName = string.Empty;
                if (_nickNamesAtPos.ContainsKey(i + 1) && _nickNamesAtPos[i + 1].Count > 0)
                {
                    normalizedNickName = NormalizedInputSelector.GetNormalizedFromInputs(_nickNamesAtPos[i + 1]);
                        
                    // Try to find similar nickname in player stats
                    var similarKey = FuzzyKeyFinder.FindSimilarKey(_playerStats.Keys, normalizedNickName, maxDistance: 2);
                    if (similarKey != null)
                    {
                        normalizedNickName = similarKey;
                    }
                }
                    
                if (!string.IsNullOrEmpty(normalizedNickName) && _playerStats.TryGetValue(normalizedNickName, out var stat))
                {
                    statsRelative.Add(stat.ToRelativeStats());
                }
            }

            return statsRelative;
        }

        #region Debbuging methods

        private void DebuggingLogs(MatchResults matchResults, PokerPosition? pokerPosition, List<bool> opponentsInGame)
        {
            if (DebugFlags.HasFlag(PokerDebugFlags.StateResults))
            {
                Log.Debug(System.Text.Json.JsonSerializer.Serialize(
                    _state.OrderBy(kvp => kvp.Key)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Results)));
            }

            if (DebugFlags.HasFlag(PokerDebugFlags.MatchResults))
            {
                Log.Debug(System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        PlayerCards = matchResults.PlayerCards.Select(c => c.ToCString()),
                        Flop = matchResults.Flop.Select(c => c.ToCString()),
                        Turn = matchResults.Turn.Select(c => c.ToCString()),
                        River = matchResults.River.Select(c => c.ToCString()),

                        OpponentCount = matchResults.Opponent.Count,
                        matchResults.Stacks,
                        matchResults.IsPlayerDecision,
                        matchResults.Pot,

                        PokerPosition = pokerPosition?.ToDisplayString() ?? "",
                        OpponentsInGame = opponentsInGame
                    }
                ));
            }
        }

        private void DebuggingLogs(GameBets gameBets)
        {
            if (DebugFlags.HasFlag(PokerDebugFlags.ActionRecognition))
            {
                Log.Debug(
                    $"Ante={gameBets.StartingBets.Ante}, SmallBlind={gameBets.StartingBets.SmallBlind} BigBlind={gameBets.StartingBets.BigBlind}");
            }
        }

        private void DebuggingLogs(IEnumerable<PlayerAction> gameBetsActions)
        {
            //Log recognized actions
            var playerActions = gameBetsActions.ToList();
            if (playerActions.Count != 0 && DebugFlags.HasFlag(PokerDebugFlags.ActionRecognition))
            {
                Log.Debug("=== Actions recognized since the start of the phase: ===");
                foreach (var action in playerActions)
                {
                    Log.Debug(action.ToString());
                }
            }
        }
        
        private void DebuggingLogs(EvResult evResult)
        {
            if (DebugFlags.HasFlag(PokerDebugFlags.Ev))
            {
                Log.Debug(evResult);
            }
        }
        
        private void DebuggingLogsForPlayerStats()
        {
            if (DebugFlags.HasFlag(PokerDebugFlags.PlayerStatistics))
            {
                Log.Debug(_playerStats.ToDebugString());
            }
        }

        #endregion
    }
}