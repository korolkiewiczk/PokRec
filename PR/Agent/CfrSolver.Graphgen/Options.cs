using CommandLine;

namespace Agent.CfrSolver.Graphgen
{
    public class Options
    {
        /// <summary>
        /// Path to json file with config. If not provided, default configuration is used.
        /// </summary>
        public string ConfigFileName { get; set; }

        /// <summary>
        /// If set, only xml is generated.
        /// </summary>
        public string XmlOnlyFile { get; set; }

        /// <summary>
        /// Number of training iterations. The default value is 5000.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Name of output table name. Default is nodes1.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Generate example config file to default.json and exit the program. It can be used to build your own configuration.
        /// </summary>
        public bool GenConfig { get; set; }

        /// <summary>
        /// No messages written on console.
        /// </summary>
        public bool Silent { get; set; }
    }
}