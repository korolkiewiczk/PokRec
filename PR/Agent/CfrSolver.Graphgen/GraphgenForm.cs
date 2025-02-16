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
using System.Linq;

namespace Agent.CfrSolver.Graphgen
{
    public partial class GraphgenForm
    {
        private const int MaxHandResolution = 16;

        private void LoadConfigValues(NodeGenConfig config)
        {
            // Set numeric values
            numSbValue.Value = config.SbValue;
            numBbValue.Value = config.BbValue;
            numReraiseAmount.Value = config.ReraiseAmount;
            numNumPlayers.Value = config.NumPlayers;
            numBankroll.Value = config.Bankroll;
            chkRelativeBetting.Checked = config.RelativeBetting;

            // Set possible raises for each phase
            for (var i = 0; i < 4; i++)
            {
                dgvPossibleRaises.Rows[i].Cells[colBets.Name].Value =
                    string.Join(", ", config.PossibleRaises[i]);
            }
        }

        private static void BrowseFile(TextBox textBox, string filter)
        {
            using var dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = dialog.FileName;
            }
        }

        private void Log(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(() => Log(message));
                return;
            }

            txtLog.AppendText(message + Environment.NewLine);
        }

        private void UpdateProgress(int value, int max)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(() => UpdateProgress(value, max));
                return;
            }

            var percent = (value * 100) / max;
            progressBar.Value = percent;
            if (value < max)
            {
                lblProgress.Text = $"{value}/{max}";
            }
            else
            {
                lblProgress.Text = string.Empty;
            }
        }

        private void BtnGenConfig_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog();
            dialog.Filter = "JSON files|*.json";
            dialog.FileName = "default.json";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(GetConfigFromControls(),
                    Formatting.Indented));
                Log($"Config file generated: {dialog.FileName}");
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
                    Iterations = (int) numIterations.Value,
                    TableName = txtTableName.Text,
                    Silent = chkSilent.Checked
                };

                await System.Threading.Tasks.Task.Run(() => ProcessMain(options));
            }
            catch (Exception ex)
            {
                Log($"Error: {ex}");
            }
            finally
            {
                btnStart.Enabled = true;
            }
        }

        private NodeGenConfig GetConfigFromControls()
        {
            var possibleRaises = new List<int[]>();
            foreach (DataGridViewRow row in dgvPossibleRaises.Rows)
            {
                var betsStr = row.Cells[colBets.Name].Value as string;
                if (!string.IsNullOrWhiteSpace(betsStr))
                {
                    var bets = betsStr.Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .ToArray();
                    possibleRaises.Add(bets);
                }
                else
                {
                    possibleRaises.Add([]);
                }
            }

            return new NodeGenConfig
            {
                SbValue = (int) numSbValue.Value,
                BbValue = (int) numBbValue.Value,
                PossibleRaises = possibleRaises.ToArray(),
                ReraiseAmount = (int) numReraiseAmount.Value,
                NumPlayers = (int) numNumPlayers.Value,
                Bankroll = (int) numBankroll.Value,
                RelativeBetting = chkRelativeBetting.Checked,
            };
        }

        private void ProcessMain(Options options)
        {
            try
            {
                btnStart.Enabled = false;
                var nodeGen = new NodeGen(GetConfigFromControls());
                txtLog.Text = string.Empty;

                TrainAndWriteToDb(nodeGen, options);
            }
            finally
            {
                btnStart.Enabled = true;
            }
        }

        private void TrainAndWriteToDb(NodeGen nodeGen, Options options)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var trainer = new TrainerParallel(nodeGen, options.Iterations, new HandGenerator(MaxHandResolution),
                new CfrPlusFactory());
            float eq;

            Log("Generating game tree...");

            var rootNode = trainer.Train(out eq, out var possibleHands,
                options.Silent ? null : x => { UpdateProgress(x + 1, options.Iterations); });
            sw.Stop();

            Log($"\nElapsed seconds on training: {(double) sw.ElapsedMilliseconds / 1000:0.##}");
            Log($"Equity: {eq}");

            var dbWriter = new DbWriter(options.TableName, () =>
                MessageBox.Show("Remove existing DB? (Y/N). If No, new table with random name will be generated.",
                    "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes);

            Log("Writing to " + dbWriter.DbName);

            for (var i = 0; i < possibleHands.Count; i++)
            {
                var hand = possibleHands.ElementAt(i);
                Log($"Hand {hand:X4}");
                dbWriter.WriteToDb(hand, rootNode, options.Silent ? null : x => Log($"Written {x} entries"));
                UpdateProgress((i + 1), possibleHands.Count);
            }

            Log("Completed");
        }

        private void GenerateXml(NodeGen nodeGen, string xmlFileName)
        {
            Log("Generating game tree...");

            var rootNode0 = nodeGen.Generate();
            XElement xElement = new XElement("Node");

            NodeTraverser.TraverseToXml(xElement, rootNode0);

            XDocument doc = new XDocument(xElement);
            doc.Save(xmlFileName);
        }

        private void btnBrowseConfig_Click(object sender, EventArgs e)
        {
            BrowseFile(txtConfigFile, "JSON files|*.json");
            if (!string.IsNullOrEmpty(txtConfigFile.Text))
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<NodeGenConfig>(File.ReadAllText(txtConfigFile.Text));
                    LoadConfigValues(config);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading config file: {ex.Message}", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private static NodeGenConfig DefaultConfig(bool relativeBetting) =>
            relativeBetting
                ? new NodeGenConfig
                {
                    SbValue = 1,
                    BbValue = 2,
                    PossibleRaises =
                    [
                        [150, 200],
                        [150, 180],
                        [200],
                        [250]
                    ],
                    ReraiseAmount = 2,
                    NumPlayers = 2,
                    Bankroll = 100,
                    RelativeBetting = true
                }
                : new NodeGenConfig
                {
                    SbValue = 1,
                    BbValue = 2,
                    PossibleRaises =
                    [
                        [4, 6],
                        [6, 12],
                        [10],
                        [15]
                    ],
                    ReraiseAmount = 1,
                    NumPlayers = 2,
                    Bankroll = 100,
                    RelativeBetting = false
                };

        private void btnOpenConfig_Click(object sender, EventArgs e)
        {
            this.tabControlMain.SelectedIndex = 1;
        }

        private void GraphgenForm_Load(object sender, EventArgs e)
        {
            // Initialize grid with predefined rows
            dgvPossibleRaises.Rows.Add("Preflop", "");
            dgvPossibleRaises.Rows.Add("Flop", "");
            dgvPossibleRaises.Rows.Add("Turn", "");
            dgvPossibleRaises.Rows.Add("River", "");

            // Load default config values
            LoadConfigValues(DefaultConfig(chkRelativeBetting.Checked));
        }

        private void btnGenXml_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog();
            dialog.Filter = "XML files|*.xml";
            dialog.FileName = "nodes.xml";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var nodeGen = new NodeGen(GetConfigFromControls());
                GenerateXml(nodeGen, dialog.FileName);
                Log($"XML file generated: {dialog.FileName}");
            }
        }

        private void chkRelativeBetting_CheckedChanged(object sender, EventArgs e)
        {
            LoadConfigValues(DefaultConfig(chkRelativeBetting.Checked));
        }
    }
}