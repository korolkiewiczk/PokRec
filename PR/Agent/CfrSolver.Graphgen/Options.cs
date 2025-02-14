using CommandLine;

namespace Agent.CfrSolver.Graphgen
{
    public class Options
    {
        /// <summary>
        /// Number of training iterations. The default value is 5000.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Name of output table name. Default is nodes1.
        /// </summary>
        public string TableName { get; init; }

        /// <summary>
        /// No detailed log
        /// </summary>
        public bool Silent { get; init; }
    }
}