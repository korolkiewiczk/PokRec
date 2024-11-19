using System.Windows.Forms;

namespace Agent;
class InputDialog
{
    public static string ShowInputDialog(string prompt)
    {
        Form promptForm = new()
        {
            Width = 300,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = prompt,
            StartPosition = FormStartPosition.CenterScreen
        };
        promptForm.Focus();
        promptForm.TopMost = true;
        TextBox textBox = new() { Left = 50, Top = 20, Width = 200 };
        Button confirmation = new() { Text = "Ok", Left = 50, Width = 100, Top = 70, DialogResult = DialogResult.OK };
        Button cancel = new() { Text = "Cancel", Left = 150, Width = 100, Top = 70, DialogResult = DialogResult.Cancel };

        promptForm.Controls.Add(textBox);
        promptForm.Controls.Add(confirmation);
        promptForm.Controls.Add(cancel);
        promptForm.AcceptButton = confirmation;
        promptForm.CancelButton = cancel;

        return promptForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
    }
}