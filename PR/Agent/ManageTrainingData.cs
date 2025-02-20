using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CfrSolver;
using Agent.CfrSolver.Graphgen; // Contains TrainingDataSerializer

namespace Agent
{
    public partial class ManageTrainingData : Form
    {
        public ManageTrainingData()
        {
            InitializeComponent();
        }

        private void ManageTrainingData_Load(object sender, EventArgs e)
        {
            LoadTrainingDataFiles();
        }

        /// <summary>
        /// Loads all .traindata files from the traindata folder.
        /// </summary>
        private void LoadTrainingDataFiles()
        {
            listBoxFiles.Items.Clear();

            // Use the application startup path to locate the traindata folder.
            string folderPath = Path.Combine(Application.StartupPath, "traindata");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Get all .traindata files.
            string[] files = Directory.GetFiles(folderPath, "*.traindata");
            listBoxFiles.Items.AddRange(files.Select(Path.GetFileNameWithoutExtension).ToArray<object>());
        }

        /// <summary>
        /// When a file is selected, load its TrainingData and display Config and Metadata in separate textboxes.
        /// </summary>
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null)
                return;

            string file = listBoxFiles.SelectedItem.ToString();
            try
            {
                // Only load Config and Metadata (skip nodes.json)
                var result = TrainingDataSerializer.LoadTrainingData(
                    file,
                    TrainingDataSerializer.DeserializeFlags.Config | TrainingDataSerializer.DeserializeFlags.Metadata);

                // Update Config fields
                if (result.Config != null)
                {
                    textBoxNumPlayers.Text = result.Config.NumPlayers.ToString();
                    textBoxSbValue.Text = result.Config.SbValue.ToString();
                    textBoxBbValue.Text = result.Config.BbValue.ToString();
                    textBoxBankroll.Text = result.Config.Bankroll.ToString();
                    textBoxReraiseAmount.Text = result.Config.ReraiseAmount.ToString();
                    textBoxRelativeBetting.Text = result.Config.RelativeBetting.ToString();
                    textBoxPossibleRaises.Text = FormatPossibleRaises(result.Config.PossibleRaises);
                }
                else
                {
                    textBoxNumPlayers.Text = "";
                    textBoxSbValue.Text = "";
                    textBoxBbValue.Text = "";
                    textBoxBankroll.Text = "";
                    textBoxReraiseAmount.Text = "";
                    textBoxRelativeBetting.Text = "";
                    textBoxPossibleRaises.Text = "";
                }

                // Update Metadata field
                if (result.Metadata != null)
                {
                    textBoxIterations.Text = result.Metadata.Iterations.ToString();
                }
                else
                {
                    textBoxIterations.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading training data:\n{ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Helper method to format the PossibleRaises array into a readable string.
        /// </summary>
        private string FormatPossibleRaises(int[][] possibleRaises)
        {
            if (possibleRaises == null)
                return "null";

            var parts = new List<string>();
            foreach (var arr in possibleRaises)
            {
                if (arr == null)
                    parts.Add("null");
                else
                    parts.Add("[" + string.Join(", ", arr) + "]");
            }

            return "[" + string.Join(", ", parts) + "]";
        }

        /// <summary>
        /// Click handler for the Open button – stubbed for future functionality.
        /// </summary>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null)
            {
                MessageBox.Show("Please select a training data file first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string file = listBoxFiles.SelectedItem.ToString();
            try
            {
                // Load the full training data including the RootNode
                var result = TrainingDataSerializer.LoadTrainingData(
                    file,
                    TrainingDataSerializer.DeserializeFlags.All);

                if (result.RootNode == null)
                {
                    MessageBox.Show("No strategy tree found in the training data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create and show the PokerStrategyForm
                var strategyForm = new PokerStrategyForm
                {
                    RootNode = result.RootNode
                };
                strategyForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading training data:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Refreshes the list of training data files.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTrainingDataFiles();
        }
    }
}