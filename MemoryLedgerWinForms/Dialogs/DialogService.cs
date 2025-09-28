using System;
using System.Drawing;
using System.Windows.Forms;

namespace MemoryLedgerWinForms.Dialogs;

internal static class DialogService
{
    public static (string Name, string Password)? PromptForDiaryCreation(IWin32Window owner)
    {
        using var form = CreateBaseForm("Crear diario");
        var nameLabel = new Label { Text = "Nombre", AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        var nameTextBox = new TextBox { Dock = DockStyle.Fill };
        var passwordLabel = new Label { Text = "Contraseña", AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        var passwordTextBox = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };

        var layout = CreateTwoColumnLayout();
        layout.Controls.Add(nameLabel, 0, 0);
        layout.Controls.Add(nameTextBox, 1, 0);
        layout.Controls.Add(passwordLabel, 0, 1);
        layout.Controls.Add(passwordTextBox, 1, 1);

        var buttons = CreateButtonsPanel();
        var okButton = new Button { Text = "Aceptar", DialogResult = DialogResult.OK, AutoSize = true };
        var cancelButton = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true };
        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(okButton);

        form.AcceptButton = okButton;
        form.CancelButton = cancelButton;
        form.Controls.Add(layout);
        form.Controls.Add(buttons);
        layout.Dock = DockStyle.Top;
        buttons.Dock = DockStyle.Bottom;
        form.Shown += (_, _) => nameTextBox.Focus();

        if (form.ShowDialog(owner) == DialogResult.OK)
        {
            return (nameTextBox.Text.Trim(), passwordTextBox.Text);
        }

        return null;
    }

    public static string? PromptForPassword(IWin32Window owner, string diaryName)
    {
        var title = $"Contraseña para '{diaryName}'";
        using var form = CreateBaseForm(title);
        var messageLabel = new Label
        {
            Text = "Introduce la contraseña",
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 8)
        };
        var passwordBox = new TextBox { Dock = DockStyle.Top, UseSystemPasswordChar = true };

        var buttons = CreateButtonsPanel();
        var okButton = new Button { Text = "Aceptar", DialogResult = DialogResult.OK, AutoSize = true };
        var cancelButton = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true };
        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(okButton);

        form.AcceptButton = okButton;
        form.CancelButton = cancelButton;
        form.Controls.Add(messageLabel);
        form.Controls.Add(passwordBox);
        form.Controls.Add(buttons);
        passwordBox.Margin = new Padding(10, 0, 10, 0);
        messageLabel.Margin = new Padding(10, 10, 10, 0);
        buttons.Dock = DockStyle.Bottom;
        form.Shown += (_, _) => passwordBox.Focus();

        return form.ShowDialog(owner) == DialogResult.OK
            ? passwordBox.Text
            : null;
    }

    private static Form CreateBaseForm(string title) => new()
    {
        Text = title,
        StartPosition = FormStartPosition.CenterParent,
        FormBorderStyle = FormBorderStyle.FixedDialog,
        MinimizeBox = false,
        MaximizeBox = false,
        ClientSize = new Size(380, 160)
    };

    private static TableLayoutPanel CreateTwoColumnLayout() => new()
    {
        ColumnCount = 2,
        RowCount = 2,
        Dock = DockStyle.Top,
        Padding = new Padding(10),
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        ColumnStyles =
        {
            new ColumnStyle(SizeType.Absolute, 110F),
            new ColumnStyle(SizeType.Percent, 100F)
        },
        RowStyles =
        {
            new RowStyle(SizeType.AutoSize),
            new RowStyle(SizeType.AutoSize)
        }
    };

    private static FlowLayoutPanel CreateButtonsPanel() => new()
    {
        FlowDirection = FlowDirection.RightToLeft,
        Padding = new Padding(10),
        Dock = DockStyle.Bottom,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink
    };
}
