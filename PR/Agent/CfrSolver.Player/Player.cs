using System;
using System.Linq;
using CfrSolver;
using CfrSolver.Datalayer;
using CfrSolver.Model;

namespace Agent.CfrSolver.Player
{
    internal class Player
    {
        private readonly string _player1DbName;
        private readonly string _player2DbName;
        private readonly bool _detailedHist;

        public bool ShowLegend { get; set; }

        private readonly Random _random = new Random();

        public Player(string player1DbName, string player2DbName, bool detailedHist)
        {
            _player1DbName = player1DbName;
            _player2DbName = player2DbName;
            _detailedHist = detailedHist;
        }

        public void Play(int currentPos0Player, ref int p0Bank, ref int p1Bank)
        {
            if (_detailedHist && ShowLegend)
            {
                Console.WriteLine("LEGEND:");
                Console.WriteLine("START Player1Hand Player2Hand WinningPlayer");
                Console.WriteLine("CurrentPlayerHand CurrentPlayer HisPosition PreviousPosition NextAction CurrentAction");
            }
            string currentAction = "R1,R1";
            int pos = 1;

            HandGenerator handGenerator = new HandGenerator(16, true);

            HandInfo handInfo= handGenerator.GenerateRandomHand();

            int winningPlayer = handInfo.WinningPlayer;
            int[] hands = handInfo.Hands;
            if (_detailedHist)
            {
                Console.WriteLine($"START\t{hands[0]:X}\t{hands[1]:X}\t{winningPlayer}");
            }

            using (DbReader dbReader1 = new DbReader(_player1DbName), dbReader2 = new DbReader(_player2DbName))
            {
                int round = 0;
                int? pay = null;
                int prevpos = -1;
                int tries = 0;
                DbReader[] dbReaders = { dbReader1, dbReader2 };

                while (pay == null)
                {
                    int currentPlayer = (1 - pos) ^ currentPos0Player;
                    if (prevpos == pos)
                    {
                        currentPlayer = pos ^ currentPos0Player;
                    }

                    int? nextPos;
                    string actions;
                    string prob;
                    if (!dbReaders[currentPlayer].GetPossibleActions(pos, hands[currentPlayer], currentAction, out nextPos, out actions,
                        out round, out prob,
                        out pay))
                    {
                        pos = 1 - pos;
                        tries++;
                        if (tries > 2)
                        {
                            if (_detailedHist)
                            {
                                Console.WriteLine("No action detected!");
                            }
                            return;
                        }
                        continue;
                    }

                    tries = 0;

                    if (nextPos != null)
                    {
                        if (pos == nextPos.Value && prevpos != pos)
                        {
                            prevpos = pos;
                            continue;
                        }

                        if (pos == nextPos.Value)
                        {
                            prevpos = -1;
                        }
                        else
                        {
                            prevpos = pos;
                        }

                        pos = nextPos.Value;
                        string nextAction = RandomAction(actions, prob);

                        if (_detailedHist)
                        {
                            Console.WriteLine($"{hands[currentPlayer]:X}\t{currentPlayer}\t{pos}\t{prevpos}\t{nextAction}\t{currentAction}");
                        }

                        currentAction += "," + nextAction;
                    }
                }

                if (round == (int)Round.Fold)
                {
                    int k = pos ^ currentPos0Player;
                    if (k == 0)
                    {
                        p0Bank -= pay.Value;
                        p1Bank += pay.Value;
                    }
                    else
                    {
                        p0Bank += pay.Value;
                        p1Bank -= pay.Value;
                    }
                }

                if (round == (int)Round.Showdown)
                {
                    int k = winningPlayer;
                    if (k == 1)
                    {
                        p0Bank -= pay.Value;
                        p1Bank += pay.Value;
                    }
                    else
                    if (k == 0)
                    {
                        p0Bank += pay.Value;
                        p1Bank -= pay.Value;
                    }
                }
            }
        }

        private int GetAction(float[] strategy)
        {
            float r = (float)_random.NextDouble();
            int a = 0;
            float cumulativeProbability = 0;
            while (a < strategy.Length - 1)
            {
                cumulativeProbability += strategy[a];
                if (r < cumulativeProbability)
                {
                    break;
                }

                a++;
            }
            return a;
        }

        private string RandomAction(string actions, string prob)
        {
            string[] actionsTab = actions.Split(',');
            float[] probTab = prob.Split(';').Select(float.Parse).ToArray();

            var action = GetAction(probTab);

            return actionsTab[action];
        }
    }
}
