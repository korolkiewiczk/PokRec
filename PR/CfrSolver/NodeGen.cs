using CfrSolver.Model;

namespace CfrSolver
{
    /// <summary>
    /// Generates poker game tree
    /// <remarks>Right now, generation is only valid for two players.</remarks>
    /// </summary>
    public class NodeGen
    {
        private readonly NodeGenConfig _config;

        public NodeGen(NodeGenConfig config)
        {
            if (config.PossibleRaises.Length != 4)
            {
                throw new Exception("Number of raises types must be 4 (PreFlop, Flop, Turn, River)");
            }
            _config = config;
        }

        public Node Generate()
        {
            int[] invest = GetStartingInvestValues();

            Node childNode = GenerateRecursive(
                player: _config.NumPlayers - 1,
                currentAction: PlayerAction.Initial(_config.BbValue-_config.SbValue), 
                round: Round.PreFlop, 
                availableReraises: _config.ReraiseAmount + 1,   //add one for preflop - bigblind can react for smallblind raise
                prevAction: PlayerAction.Initial(_config.SbValue), 
                pot: _config.SbValue + _config.BbValue, 
                invest: invest);

            Node rootNode = new Node(
                pos: (byte)(_config.NumPlayers - 2), 
                action: PlayerAction.Initial(_config.SbValue), 
                round: Round.PreFlop, 
                children: [childNode], 
                payOff: invest.Max());

            return rootNode;
        }

        private int[] GetStartingInvestValues()
        {
            int[] invest = new int[_config.NumPlayers];
            invest[_config.NumPlayers - 1] = _config.BbValue;
            invest[_config.NumPlayers - 2] = _config.SbValue;
            return invest;
        }

        /// <summary>
        /// Generate poker game tree recursive
        /// </summary>
        /// <param name="player">Current player for which sub tree is generated. 0 is dealer, max is small blind</param>
        /// <param name="currentAction">Currently performed action</param>
        /// <param name="round">Current round</param>
        /// <param name="availableReraises">Number of available reraises. If zero, only Fold/Call/All actions are possible</param>
        /// <param name="prevAction">Previous action</param>
        /// <param name="pot">Current deal pot</param>
        /// <param name="invest">How much players invested</param>
        /// <returns>Node with recursively generated child nodes for subtree game states</returns>
        private Node GenerateRecursive(int player, PlayerAction currentAction, Round round, int availableReraises, PlayerAction prevAction, int pot, int[] invest)
        {
            if (IsSmallblindLimpedBigBlindOnPreflop(player, currentAction, round))
            {
                availableReraises = _config.ReraiseAmount;
            }

            var willBeNewRound = false;
            if (GetNewRoundBasingOnCurrentActions(player, round, currentAction, prevAction, availableReraises,
                    out var newRoundBasingOnCurrentActions))
            {
                willBeNewRound = newRoundBasingOnCurrentActions != round;
            }
            //todo BUG - player=1 has only one CALL action in new section when betting was previously performed
            PlayerAction[] possibleActions = GetPossibleActions(round, availableReraises, currentAction, pot, willBeNewRound);

            List<Node> nodes = new List<Node>(possibleActions.Length);
            foreach (var possibleAction in possibleActions)
            {
                PlayerAction newCurrentAction = SaturateBet(invest, possibleAction);
                if (newCurrentAction.Equals(PlayerAction.Invalid))
                {
                    continue;
                }

                int[] newInvest = GetNewInvest(invest);
                int investValue = GetCurrentActionInvestValue(newCurrentAction, currentAction);     //invest raises by current action bet

                var newRound = GetNewRound(player, possibleAction, round, currentAction, prevAction, availableReraises);

                bool isNewRound = newRound < Round.Showdown && newRound != round;

                int nextPlayerPosition = NextPlayerPosition(player, isNewRound);
                newInvest[nextPlayerPosition] += investValue;   //next player invest raises by his bet
                int reraiseDiff = currentAction.OpType == OpType.Raise ? 1 : 0; //if raise action is performed, number of possible reraises is decreased by one

                var newReraiseAmount = isNewRound ? _config.ReraiseAmount : availableReraises - reraiseDiff;
                var newPot = pot + investValue;

                nodes.Add(GenerateRecursive(nextPlayerPosition, newCurrentAction, newRound, newReraiseAmount,
                    currentAction, //currentAction becomes prevAction
                    newPot, newInvest));
            }

            return new Node((byte)player, currentAction, round, nodes.ToArray(),
                GetPayOff(player, round, pot, invest));
        }

        private static int[] GetNewInvest(int[] invest)
        {
            int[] newInvest = new int[invest.Length];
            Array.Copy(invest, newInvest, invest.Length);
            return newInvest;
        }

        /// <summary>
        /// Situation when Small blind player has called (limped) Bigblind. Bigblind can perform betting action
        /// </summary>
        private static bool IsSmallblindLimpedBigBlindOnPreflop(int player, PlayerAction currentAction, Round round)
        {
            return player == 1 && currentAction.OpType == OpType.Call && round == Round.PreFlop;
        }

        /// <summary>
        /// Cut bet to it's max value
        /// </summary>
        private PlayerAction SaturateBet(int[] invest, PlayerAction possibleAction)
        {
            if (possibleAction.OpType == OpType.All)
            {
                return new PlayerAction(OpType.All, possibleAction.Bet - invest.Max());
            }

            if (possibleAction.OpType == OpType.Raise 
                && possibleAction.Bet + invest.Max() > _config.Bankroll)  //when possible raise is bigger than current player bankroll
            {
                return PlayerAction.Invalid;  //since All action is always included, we will skip this action at all
            }

            return possibleAction;
        }

        private static int GetPayOff(int player, Round round, int pot, int[] invest)
        {
            if (Node.IsTerminal(round))
            {
                if (round == Round.Fold)
                {
                    invest[player] += pot;
                }
                return invest.Select(x => pot - x).Max();   //note that player wins pot but he invested x before.
            }

            return 0;
        }

        private int GetCurrentActionInvestValue(PlayerAction newAction, PlayerAction currentAction)
        {
            switch (newAction.OpType)
            {
                case OpType.Fold:
                    return 0;
                case OpType.Call:
                    return currentAction.Bet;
                case OpType.Raise:
                case OpType.All:
                    return currentAction.Bet + newAction.Bet;
            }

            return 0;
        }

        /// <summary>
        /// Gets new round according to current game state and betting
        /// </summary>
        /// <param name="player">Parent player performing action. He performed <see cref="currentAction"/></param>
        /// <param name="newAction">Newly performed action for next player. It can be the same player if new round starts. Decided after.</param>
        /// <param name="round">Current round (on which <see cref="player"/> is</param>
        /// <param name="currentAction">Current action (which <see cref="player"/> performs)</param>
        /// <param name="prevAction">Previous action</param>
        /// <param name="availableReraises">Available amount of reraise</param>
        /// <returns>New round for next player</returns>
        private Round GetNewRound(int player, PlayerAction newAction, Round round, PlayerAction currentAction, PlayerAction prevAction, int availableReraises)
        {
            if (newAction.OpType == OpType.Fold)    //if we folds, go to Fold round
            {
                return Round.Fold;
            }

            if (newAction.OpType == OpType.Call && currentAction.OpType == OpType.All   //call all-in => Showdown
                || round == Round.River && NextPlayerPosition(player) == 0 && newAction.OpType == OpType.Call   //Showdown after river & dealer player
                || round == Round.River && NextPlayerPosition(player) == 1 
                                        && newAction.OpType == OpType.Call && currentAction.OpType == OpType.Raise)  //dealer raises, next player calls - Showdown
            {
                return Round.Showdown;
            }

            if (GetNewRoundBasingOnCurrentActions(player, round, currentAction, prevAction, availableReraises, out var newRound)) 
                return newRound;

            return round;
        }

        private bool GetNewRoundBasingOnCurrentActions(int player, Round round, PlayerAction currentAction,
            PlayerAction prevAction, int availableReraises, out Round newRound)
        {
            if (round > Round.PreFlop)
            {
                if (player == 0 && currentAction.OpType == OpType.Call  //dealer only calls => next round
                    || player == 1 && currentAction.OpType == OpType.Call && prevAction.OpType == OpType.Raise) //last to decision player calls after raise => next round
                {
                    newRound = round + 1;
                    return true;
                }
            }
            else  //preflop
            {
                if (player == 1 && currentAction.OpType == OpType.Call  //last-to-decision player only calls => Flop
                    || player == 0 && currentAction.OpType == OpType.Call && prevAction.OpType == OpType.Raise && availableReraises != _config.ReraiseAmount) //dealer calls => Flop
                {
                    newRound = Round.Flop;
                    return true;
                }
            }

            newRound = round;
            return false;
        }

        /// <summary>
        /// Gets possible actions for current game state. They will be iterated for child actions
        /// </summary>
        /// <param name="round">Current round</param>
        /// <param name="availableReraises">Available reraises. If zero, only Fold/Call/All actions are possible</param>
        /// <param name="currentAction">Current action</param>
        /// <param name="pot">Current deal pot</param>
        /// <returns>Possible actions for current state</returns>
        private PlayerAction[] GetPossibleActions(Round round, int availableReraises, PlayerAction currentAction, int pot, bool willBeNewRound)
        {
            if (round == Round.Fold || round == Round.Showdown) //for terminal state, no actions are permited
            {
                return new PlayerAction[0];
            }

            if (currentAction.OpType == OpType.All) //player can only Fold or check all-in
            {
                return new[]
                {
                    new PlayerAction(OpType.Fold),
                    new PlayerAction(OpType.Call)
                };
            }

            if (availableReraises <= 0 && !willBeNewRound) //no available reraises - just call or fold
            {
                if (currentAction.OpType == OpType.Call)
                {
                    return new[]
                    {
                        new PlayerAction(OpType.Call)
                    };
                }

                return new[]
                {
                    new PlayerAction(OpType.Fold),
                    new PlayerAction(OpType.Call)
                };
            }

            var actions = new List<PlayerAction>  //base actions
            {
                new PlayerAction(OpType.Call),
                new PlayerAction(OpType.All, _config.Bankroll)
            };

            if (currentAction.OpType != OpType.Call)    //we can also fold if Raise was done before
            {
                actions.Insert(0, new PlayerAction(OpType.Fold));
            }

            //beting
            if (_config.RelativeBetting)
            {
                actions.AddRange(_config.PossibleRaises[(int)round]
                    .Select(possibleRaise => new PlayerAction(OpType.Raise, GetRelativeBet(pot, possibleRaise))));
            }
            else
            {
                actions.AddRange(_config.PossibleRaises[(int)round]
                    .Select(possibleRaise => new PlayerAction(OpType.Raise, possibleRaise)));
            }

            return actions.ToArray();
        }

        private static int GetRelativeBet(int pot, int raise)
        {
            return raise * pot / 100;
        }

        private int NextPlayerPosition(int player, bool isNewRound = false)
        {
            return isNewRound ? _config.NumPlayers - 1 : (player == 0 ? _config.NumPlayers - 1 : player - 1);
        }
    }
}
