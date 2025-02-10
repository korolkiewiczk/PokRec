using CommandLine;

namespace Agent.CfrSolver.Player
{
    public class Options
    {
        [Option('a', "dbName1", Required = true, HelpText = "Name of first player db.")]
        public string DbName1 { get; set; }

        [Option('b', "dbName2", Required = true, HelpText = "Name of second player db.")]
        public string DbName2 { get; set; }

        [Option('t', "table", Required = false, HelpText = "Table output.")]
        public bool Table { get; set; }

        [Option('h', "history", Required = false, HelpText = "Detailed playing history.")]
        public bool DetailedHistory { get; set; }
    }
}
