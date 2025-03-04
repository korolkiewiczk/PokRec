using System;
using System.Linq;
using System.Windows.Forms;
using CfrSolver.Model;    // Contains Node and related classes.
using CfrSolver;          // Contains BoardGenerator.
using PT.Poker.Model;
using System.Collections.Generic;
using System.Text;
using CfrSolver.Utils; // Contains Card, CardType, CardColor.

namespace Agent
{
    public partial class PokerStrategyForm : Form
    {
        /// <summary>
        /// The root of the CFR tree.
        /// This should be set externally from your application.
        /// </summary>
        public Node RootNode { get; set; }
        
        public PokerStrategyForm()
        {
            InitializeComponent();
            // Populate the card ComboBoxes.
            PopulateCards(comboPlayerHand1);
            PopulateCards(comboPlayerHand2);
            PopulateCards(comboFlop1);
            PopulateCards(comboFlop2);
            PopulateCards(comboFlop3);
            PopulateCards(comboTurn);
            PopulateCards(comboRiver);
            
            // Populate the listBoxActions with possible actions.
            
            
            this.dataGridViewStrategy.Columns[0].Name = "Action";
            this.dataGridViewStrategy.Columns[1].Name = "Strategy (%)";
        }
        
        /// <summary>
        /// Fills the provided ComboBox with a list of cards (e.g. "2♣", "J♦", "A♠", etc.).
        /// </summary>
        private void PopulateCards(ComboBox combo)
        {
            string[] types = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            string[] suits = { "♣", "♦", "♥", "♠" };
            combo.Items.Clear();
            combo.Items.Add("");
            foreach (var type in types)
            {
                foreach (var suit in suits)
                {
                    combo.Items.Add(type + suit);
                }
            }
            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }
        
        /// <summary>
        /// Converts a string (like "A♠") into a Card instance.
        /// </summary>
        private Card ConvertToCard(string cardStr)
        {
            char suitChar = cardStr[^1];
            CardColor color;
            switch (suitChar)
            {
                case '♣':
                    color = CardColor.Clubs;
                    break;
                case '♦':
                    color = CardColor.Diamonds;
                    break;
                case '♥':
                    color = CardColor.Hearts;
                    break;
                case '♠':
                    color = CardColor.Spades;
                    break;
                default:
                    throw new ArgumentException("Invalid card suit.");
            }
            string valueStr = cardStr[..^1];
            CardType type;
            switch (valueStr)
            {
                case "2": type = CardType.C2; break;
                case "3": type = CardType.C3; break;
                case "4": type = CardType.C4; break;
                case "5": type = CardType.C5; break;
                case "6": type = CardType.C6; break;
                case "7": type = CardType.C7; break;
                case "8": type = CardType.C8; break;
                case "9": type = CardType.C9; break;
                case "10": type = CardType.C10; break;
                case "J": type = CardType.J; break;
                case "Q": type = CardType.Q; break;
                case "K": type = CardType.K; break;
                case "A": type = CardType.A; break;
                default:
                    throw new ArgumentException("Invalid card value.");
            }
            return new Card(color, type);
        }
        
        
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateStrategy();
        }
        
        /// <summary>
        /// Handles the Calculate Strategy button click.
        /// Gathers card selections, builds the board abstraction, traverses the CFR tree based on selected actions,
        /// and displays the average strategy as percentages.
        /// </summary>
        private void CalculateStrategy()
        {
            try
            {
                // Build player's hand.
                Card[] playerHand = new Card[2];
                if (comboPlayerHand1.SelectedItem != null && comboPlayerHand1.SelectedItem.ToString() != "" &&
                    comboPlayerHand2.SelectedItem != null && comboPlayerHand2.SelectedItem.ToString() != "")
                {
                    playerHand[0] = ConvertToCard(comboPlayerHand1.SelectedItem.ToString());
                    playerHand[1] = ConvertToCard(comboPlayerHand2.SelectedItem.ToString());
                }
                else
                {
                    MessageBox.Show("Select at least player cards.");
                    return;
                }

                // Build flop if available.
                Card[] flop = null;
                if (comboFlop1.SelectedItem != null && comboFlop1.SelectedItem.ToString() != "" &&
                    comboFlop2.SelectedItem != null && comboFlop2.SelectedItem.ToString() != "" &&
                    comboFlop3.SelectedItem != null && comboFlop3.SelectedItem.ToString() != "")
                {
                    flop = new Card[3];
                    flop[0] = ConvertToCard(comboFlop1.SelectedItem.ToString());
                    flop[1] = ConvertToCard(comboFlop2.SelectedItem.ToString());
                    flop[2] = ConvertToCard(comboFlop3.SelectedItem.ToString());
                }
                
                // Get turn card.
                Card[] turn = null;
                if (comboTurn.SelectedItem != null && comboTurn.SelectedItem.ToString() != "")
                {
                    turn = [ConvertToCard(comboTurn.SelectedItem.ToString())];
                }
                
                // Get river card.
                Card[] river = null;
                if (comboRiver.SelectedItem != null && comboRiver.SelectedItem.ToString() != "")
                {
                    river = [ConvertToCard(comboRiver.SelectedItem.ToString())];
                }
                // Create a list of all selected cards
                var allCards = new List<Card>();
                allCards.AddRange(playerHand);
                if (flop != null) allCards.AddRange(flop);
                if (turn != null) allCards.AddRange(turn);
                if (river != null) allCards.AddRange(river);

                // Check for duplicates
                var duplicates = allCards.GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicates.Any())
                {
                    MessageBox.Show($"Duplicate cards detected: {string.Join(", ", duplicates)}. Each card can only be used once.");
                    return;
                }
                // Generate board abstraction using the provided cards.
                BoardGenerator generator = new BoardGenerator(16);
                var boardInfo = generator.GenerateBoardAbstraction(playerHand, flop, turn, river);
                int handAbstraction = boardInfo.Hand;

                string rootPreffix = RootNode.Action.ToShortString();
                if (RootNode.Children.Length == 1)
                {
                    rootPreffix = $"{rootPreffix},{RootNode.Children[0].Action.ToShortString()}";
                }
                
                // Get action sequence from listBoxActions.
                var selectedItems = txtActions.Text;
                if (string.IsNullOrWhiteSpace(selectedItems))
                {
                    selectedItems = rootPreffix;
                }
                else
                {
                    selectedItems = $"{rootPreffix},{selectedItems}";
                }
                string[] actions = selectedItems.Split(",").Skip(1).ToArray();

                if (RootNode == null)
                {
                    MessageBox.Show("CFR tree root node is not set.");
                    return;
                }
                // Traverse the CFR tree using the action sequence.
                Node selectedNode = RootNode.GetNodeForActionSequence(actions);
                if (selectedNode == null)
                {
                    MessageBox.Show("No matching node found for the selected actions.");
                    return;
                }
                
                // Retrieve the average strategy for the given hand abstraction.
                float[] strategy = selectedNode.GetAverageStrategy(handAbstraction);
                
                // Display the strategy in the DataGridView.
                dataGridViewStrategy.Rows.Clear();
                for (int i = 0; i < strategy.Length; i++)
                {
                    string actionStr = (selectedNode.Children != null && selectedNode.Children.Length > i)
                        ? selectedNode.Children[i].Action.ToShortString()
                        : "N/A";
                    string percent = (strategy[i] * 100).ToString("0.00") + "%";
                    dataGridViewStrategy.Rows.Add(actionStr, percent);
                }

                // Generate a detailed description of the current game state.
                var stateDescription = new StringBuilder();

// Add information about the current position and round.
                stateDescription.AppendLine($"Last Player: {selectedNode.Pos}");
                stateDescription.AppendLine($"Last Round: {selectedNode.Round}");

// Add information about the actions taken before reaching this node.
                stateDescription.AppendLine("Actions leading to this state:");
                if (selectedItems.Length > 0)
                {
                    stateDescription.AppendLine(selectedItems);
                }

// Add information about the hand abstraction.
                stateDescription.AppendLine($"Hand Abstraction: 0x{handAbstraction:X4}");

// Add information about the pay-off if it’s a terminal state.
                if (selectedNode.IsTerminal())
                {
                    stateDescription.AppendLine($"Terminal State: {selectedNode.Round}");
                    stateDescription.AppendLine($"Payoff: {selectedNode.PayOff}");
                }
                else
                {
                    stateDescription.AppendLine("Non-terminal state. Next possible actions:");
                    if (selectedNode.Children is {Length: > 0})
                    {
                        foreach (var child in selectedNode.Children)
                        {
                            stateDescription.AppendLine($"- Action: {child.Action.ToShortString()} (Player: {child.Pos}, Round: {child.Round})");
                        }
                    }
                    else
                    {
                        stateDescription.AppendLine("No further actions available.");
                    }
                }

                stateDescription.AppendLine();
                stateDescription.AppendLine("LEGEND:");
                stateDescription.AppendLine("Pos: The player’s position in the game tree.");
                stateDescription.AppendLine("Round: The current stage of the game (PreFlop, Flop, Turn, River, Showdown, Fold).");
                stateDescription.AppendLine("Actions: The available moves a player can make: Fold (F), Call (C), Raise N (R{N}), All-In N (A{N}). N is what is additionally added to current stack.");
                if (RootNode.Children.Length == 1)
                {
                    stateDescription.AppendLine("First two actions are Raise SB, Raise (BB-SB).");
                }
                stateDescription.AppendLine("Hand Abstraction: A hexadecimal value representing the hand state (e.g., 0x4321 means 4 for River, 3 for Turn, 2 for Flop, and 1 for Player Cards).");
                stateDescription.AppendLine("Payoff: The numerical outcome (gain/loss) when a terminal state is reached.");
                stateDescription.AppendLine();
                stateDescription.AppendLine("HINT: Double click on table with possible actions to select action and compute");

                // Assign the generated description to the label.
                lblState.Text = stateDescription.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridViewStrategy_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var item = dataGridViewStrategy.Rows[e.RowIndex].Cells[0].Value.ToString()!;
                if (string.IsNullOrWhiteSpace(txtActions.Text))
                    txtActions.Text = item;
                else
                    txtActions.Text += $",{item}";
                CalculateStrategy();
            }
            catch
            {
                // ignored
            }
        }
    }
}
