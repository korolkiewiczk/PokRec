using System.Windows.Forms;

namespace Agent;

static class InputDialog
{
    public static string ShowInputDialog(string prompt)
    {
        Form promptForm = ConfigureForm(prompt);
        TextBox textBox = ConfigureTextBox();
        Button confirmation = ConfigureConfirmationButton();
        Button cancel = ConfigureCancelButton();

        AddControls(promptForm, textBox, confirmation, cancel);

        return promptForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
    }

    private static Form ConfigureForm(string prompt)
    {
        return new Form
        {
            Width = 300,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = prompt,
            StartPosition = FormStartPosition.CenterScreen,
            TopMost = true
        };
    }

    private static TextBox ConfigureTextBox()
    {
        return new TextBox { Left = 50, Top = 20, Width = 200 };
    }

    private static Button ConfigureConfirmationButton()
    {
        return new Button { Text = "Ok", Left = 50, Width = 100, Top = 70, DialogResult = DialogResult.OK };
    }

    private static Button ConfigureCancelButton()
    {
        return new Button { Text = "Cancel", Left = 150, Width = 100, Top = 70, DialogResult = DialogResult.Cancel };
    }

    private static void AddControls(Form form, TextBox textBox, Button confirmation, Button cancel)
    {
        form.Controls.Add(textBox);
        form.Controls.Add(confirmation);
        form.Controls.Add(cancel);
        form.AcceptButton = confirmation;
        form.CancelButton = cancel;
    }
}