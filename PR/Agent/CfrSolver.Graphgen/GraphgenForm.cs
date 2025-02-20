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
using CfrSolver.Model;

namespace Agent.CfrSolver.Graphgen
{
    public partial class GraphgenForm
    {
        private const int MaxHandResolution = 16;
        private System.Threading.CancellationTokenSource _cancellationTokenSource;
        private DateTime _lastMeasureTime;
        private int _lastMeasureValue;
        private double _averageTimePerThousand;

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

            var percent = value * 100 / max;
            progressBar.Value = percent;

            // Measure time every 1000 operations
            if (value % 1000 == 0)
            {
                if (_lastMeasureTime != default)
                {
                    var timeDiff = (DateTime.Now - _lastMeasureTime).TotalSeconds;
                    var valueDiff = value - _lastMeasureValue;
                    _averageTimePerThousand = (timeDiff / valueDiff) * 1000;
                }
                _lastMeasureTime = DateTime.Now;
                _lastMeasureValue = value;
            }

            if (value < max)
            {
                var remainingOperations = max - value;
                var estimatedSeconds = (_averageTimePerThousand * remainingOperations) / 1000;
                var timeToComplete = TimeSpan.FromSeconds(estimatedSeconds);
                
                string timeInfo = _averageTimePerThousand > 0 
                    ? $"\n({timeToComplete:hh\\:mm\\:ss} to complete, {_averageTimePerThousand:F1}s/1000 ops)"
                    : "";
                    
                lblProgress.Text = $"{value}/{max}{timeInfo}";
            }
            else
            {
                lblProgress.Text = string.Empty;
                // Reset measurement values
                _lastMeasureTime = default;
                _lastMeasureValue = 0;
                _averageTimePerThousand = 0;
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
            if (btnStart.Text == "Stop")
            {
                _cancellationTokenSource?.Cancel();
                return;
            }

            var prevBtnText = btnStart.Text;
            try
            {
                progressBar.Value = 0;
                btnStart.Text = "Stop";
                _cancellationTokenSource = new System.Threading.CancellationTokenSource();
                await System.Threading.Tasks.Task.Run(() => ProcessMain(_cancellationTokenSource.Token));
            }
            catch (OperationCanceledException)
            {
                Log("Process cancelled by user");
            }
            catch (Exception ex)
            {
                Log($"Error: {ex}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                btnStart.Text = prevBtnText;
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

        private void ProcessMain(System.Threading.CancellationToken cancellationToken)
        {
            var config = GetConfigFromControls();
            var nodeGen = new NodeGen(config);
            txtLog.Text = string.Empty;

            var rootNode = TrainAndWriteToDb(nodeGen, cancellationToken);
            
            // Save training data
            try
            {
                TrainingDataSerializer.SaveTrainingData(
                    txtTableName.Text,
                    rootNode,
                    config,
                    (int)numIterations.Value
                );
                Log($"Training data saved to {txtTableName.Text}.traindata");
            }
            catch (Exception ex)
            {
                Log($"Error saving training data: {ex.Message}");
            }
        }

        private Node TrainAndWriteToDb(NodeGen nodeGen, System.Threading.CancellationToken cancellationToken)
        {
            Stopwatch sw = new Stopwatch();

            int iterations = (int)numIterations.Value;
            sw.Start();
            var trainer = new TrainerParallel(nodeGen, iterations, new HandGenerator(MaxHandResolution),
                new CfrPlusFactory());

            Log("Generating game tree...");

            var rootNode = trainer.Train(out var eq, out var possibleHands,
                x =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    UpdateProgress(x + 1, iterations);
                },
                cancellationToken);
            sw.Stop();

            Log($"\nElapsed seconds on training: {(double)sw.ElapsedMilliseconds / 1000:0.##}");
            Log($"Equity: {eq}");

            if (chkIncludeDb.Checked) 
            {
                SaveNodesToDatabase(possibleHands, rootNode, cancellationToken);
            }
            
            return rootNode; // Return rootNode for serialization
        }

        private void SaveNodesToDatabase(HashSet<int> possibleHands, Node rootNode,
            System.Threading.CancellationToken cancellationToken)
        {
            var dbWriter = new DbWriter(txtTableName.Text, () =>
                MessageBox.Show("Remove existing DB? (Y/N). If No, new table with random name will be generated.",
                    "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes);

            Log("Writing to " + dbWriter.DbName);

            for (var i = 0; i < possibleHands.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hand = possibleHands.ElementAt(i);
                Log($"Hand {hand:X4}");
                dbWriter.WriteToDb(hand, rootNode, x => Log($"Written {x} entries"));
                UpdateProgress(i + 1, possibleHands.Count);
                lblProgress.Text = "Saving to db " + lblProgress.Text;
            }

            Log("Completed. Checkout cfr.db");
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
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Config files (*.json;*.traindata)|*.json;*.traindata|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        txtConfigFile.Text = openFileDialog.FileName;
                        NodeGenConfig config;
                        string extension = Path.GetExtension(openFileDialog.FileName).ToLower();

                        if (extension == ".traindata")
                        {
                            var (_, loadedConfig, _) = TrainingDataSerializer.LoadTrainingData(
                                openFileDialog.FileName, 
                                TrainingDataSerializer.DeserializeFlags.Config
                            );
                            config = loadedConfig;
                        }
                        else
                        {
                            // Original JSON loading logic
                            string jsonString = File.ReadAllText(openFileDialog.FileName);
                            config = JsonConvert.DeserializeObject<NodeGenConfig>(jsonString);
                        }

                        if (config != null)
                        {
                            LoadConfigValues(config);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading config: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            
            // Set initial table name
            UpdateTableName();
            
            // Add event handlers for numeric controls
            numBankroll.ValueChanged += (s, ev) => UpdateTableName();
            numBbValue.ValueChanged += (s, ev) => UpdateTableName();
        }

        private void UpdateTableName()
        {
            txtTableName.Text = $"nodes_{(int)Math.Floor(numBankroll.Value/numBbValue.Value)}BB";
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