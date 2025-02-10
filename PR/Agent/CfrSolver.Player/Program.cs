using System;
using CommandLine;

namespace Agent.CfrSolver.Player
{
    internal class Program
    {
        static void _Main(string[] args)
        {
            int p0Bank = 0, p1Bank = 0;

            int currentPos0Player = 0;

            int iter = 1;

            Options options = null;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => options = o)
                .WithNotParsed(err=> Environment.Exit(-1));

            Player player = new Player(options.DbName1, options.DbName2, options.DetailedHistory)
            {
                ShowLegend = true
            };

            do
            {
                while (!Console.KeyAvailable)
                {
                    player.Play(currentPos0Player, ref p0Bank, ref p1Bank);

                    if (options.Table)
                    {
                        Console.WriteLine($"{iter}\t{currentPos0Player}\t{p0Bank}\t{p1Bank}");
                    }
                    else
                    {
                        Console.WriteLine($"{iter}. Pos={currentPos0Player} P1={p0Bank} P2={p1Bank}");
                    }

                    currentPos0Player = 1 - currentPos0Player;

                    iter++;

                    player.ShowLegend = false;
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
