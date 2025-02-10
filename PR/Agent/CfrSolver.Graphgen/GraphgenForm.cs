using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using CfrSolver;
using CfrSolver.Cfr;
using CfrSolver.Datalayer;
using CfrSolver.Utils;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Agent.CfrSolver.Graphgen
{
    public partial class GraphgenForm : Form
    {
        private const int MaxHandResolution = 16;

        public GraphgenForm()
        {
            InitializeComponent();
        }

        private void BrowseFile(TextBox textBox, string filter)
        {
            using (var dialog = new OpenFileDialog { Filter = filter })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = dialog.FileName;
                }
            }
        }

        private void Log(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(message)));
                return;
            }

            txtLog.AppendText(message + Environment.NewLine);
        }

        private void UpdateProgress(int value)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => UpdateProgress(value)));
                return;
            }

            progressBar.Value = value;
        }

        private void BtnGenConfig_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "JSON files|*.json", FileName = "default.json" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(DefaultConfig, Formatting.Indented));
                    Log($"Config file generated: {dialog.FileName}");
                }
            }
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            progressBar.Value = 0;

            try
            {
                var options = new Options
                {
                    ConfigFileName = string.IsNullOrEmpty(txtConfigFile.Text) ? null : txtConfigFile.Text,
                    XmlOnlyFile = string.IsNullOrEmpty(txtXmlFile.Text) ? null : txtXmlFile.Text,
                    Iterations = (int)numIterations.Value,
                    TableName = txtTableName.Text,
                    Silent = chkSilent.Checked
                };

                await System.Threading.Tasks.Task.Run(() => ProcessMain(options));
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
            finally
            {
                btnStart.Enabled = true;
            }
        }

        private void ProcessMain(Options options)
        {
            NodeGen nodeGen = null;
            bool onlyXml = false;
            string xmlFileName = "";

            NodeGenConfig defaultConfig = DefaultConfig;

            if (options.ConfigFileName != null)
            {
                var nodeGenConfig = JsonConvert.DeserializeObject<NodeGenConfig>(File.ReadAllText(options.ConfigFileName));
                nodeGen = new NodeGen(nodeGenConfig);
            }
            else
            {
                nodeGen = new NodeGen(defaultConfig);
            }

            if (options.XmlOnlyFile != null)
            {
                onlyXml = true;
                xmlFileName = options.XmlOnlyFile;
            }

            if (onlyXml)
            {
                GenerateXml(nodeGen, xmlFileName, options);
                return;
            }

            TrainAndWriteToDb(nodeGen, options);
        }

        private void TrainAndWriteToDb(NodeGen nodeGen, Options options)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var trainer = new Trainer(nodeGen, options.Iterations, new HandGenerator(MaxHandResolution), new CfrPlusFactory());
            float eq;
            HashSet<int> possibleHands;

            if (!options.Silent)
            {
                Log("Generating game tree...");
            }

            var rootNode = trainer.Train(out eq, out possibleHands, options.Silent ? null : x =>
            {
                UpdateProgress(x * 100 / options.Iterations);
            });
            sw.Stop();

            if (!options.Silent)
            {
                Log($"\nElapsed seconds on training: {(double)sw.ElapsedMilliseconds / 1000:0.##}");
                Log($"Equity: {eq}");
            }

            var dbWriter = new DbWriter(options.TableName, () => 
                MessageBox.Show("Remove existing DB? (Y/N). If No, new table with random name will be generated.", 
                    "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes);

            if (!options.Silent)
            {
                Log("Writing to " + dbWriter.DbName);
            }

            foreach (var hand in possibleHands)
            {
                Log($"Hand {hand:X4}");
                dbWriter.WriteToDb(hand, rootNode, options.Silent ? null : x => Log($"Written {x} entries"));
            }
        }

        private void GenerateXml(NodeGen nodeGen, string xmlFileName, Options options)
        {
            if (!options.Silent)
            {
                Log("Generating game tree...");
            }

            var rootNode0 = nodeGen.Generate();
            XElement xElement = new XElement("Node");

            NodeTraverser.TraverseToXml(xElement, rootNode0);

            XDocument doc = new XDocument(xElement);
            doc.Save(xmlFileName);
        }


        public static NodeGenConfig DefaultConfig => new NodeGenConfig
        {
            SbValue = 1,
            BbValue = 2,
            PossibleRaises = new[]
            {
                new[] {4, 6},
                new[] {6, 12},
                new[] {10, 20},
                new[] {15, 25}
            },
            ReraiseAmount = 1,
            NumPlayers = 2,
            Bankroll = 100
        };
    }
} 