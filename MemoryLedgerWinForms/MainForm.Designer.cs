using System.Windows.Forms;

namespace MemoryLedgerWinForms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        mainLayout = new TableLayoutPanel();
        diariesGroupBox = new GroupBox();
        diariesLayout = new TableLayoutPanel();
        diariesListBox = new ListBox();
        diaryButtonsPanel = new FlowLayoutPanel();
        refreshDiariesButton = new Button();
        openDiaryButton = new Button();
        createDiaryButton = new Button();
        deleteDiaryButton = new Button();
        entriesGroupBox = new GroupBox();
        entriesLayout = new TableLayoutPanel();
        currentDiaryLabel = new Label();
        entriesListView = new ListView();
        columnDate = new ColumnHeader();
        columnTitle = new ColumnHeader();
        columnIntensity = new ColumnHeader();
        entryEditorLayout = new TableLayoutPanel();
        entryDateLabel = new Label();
        entryDatePicker = new DateTimePicker();
        entryTitleLabel = new Label();
        entryTitleTextBox = new TextBox();
        entryDescriptionLabel = new Label();
        entryDescriptionTextBox = new TextBox();
        entryIntensityLabel = new Label();
        entryIntensityNumericUpDown = new NumericUpDown();
        entryButtonsPanel = new FlowLayoutPanel();
        addEntryButton = new Button();
        updateEntryButton = new Button();
        deleteEntryButton = new Button();
        statisticsButton = new Button();
        statusLabel = new Label();
        mainLayout.SuspendLayout();
        diariesGroupBox.SuspendLayout();
        diariesLayout.SuspendLayout();
        diaryButtonsPanel.SuspendLayout();
        entriesGroupBox.SuspendLayout();
        entriesLayout.SuspendLayout();
        entryEditorLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)entryIntensityNumericUpDown).BeginInit();
        entryButtonsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // mainLayout
        // 
        mainLayout.ColumnCount = 2;
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayout.Controls.Add(diariesGroupBox, 0, 0);
        mainLayout.Controls.Add(entriesGroupBox, 1, 0);
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.Location = new System.Drawing.Point(0, 0);
        mainLayout.Name = "mainLayout";
        mainLayout.RowCount = 1;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.Size = new System.Drawing.Size(1084, 661);
        mainLayout.TabIndex = 0;
        // 
        // diariesGroupBox
        // 
        diariesGroupBox.Controls.Add(diariesLayout);
        diariesGroupBox.Dock = DockStyle.Fill;
        diariesGroupBox.Location = new System.Drawing.Point(3, 3);
        diariesGroupBox.Name = "diariesGroupBox";
        diariesGroupBox.Padding = new Padding(10);
        diariesGroupBox.Size = new System.Drawing.Size(254, 655);
        diariesGroupBox.TabIndex = 0;
        diariesGroupBox.TabStop = false;
        diariesGroupBox.Text = "Diarios";
        // 
        // diariesLayout
        // 
        diariesLayout.ColumnCount = 1;
        diariesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        diariesLayout.Controls.Add(diariesListBox, 0, 0);
        diariesLayout.Controls.Add(diaryButtonsPanel, 0, 1);
        diariesLayout.Dock = DockStyle.Fill;
        diariesLayout.Location = new System.Drawing.Point(10, 26);
        diariesLayout.Name = "diariesLayout";
        diariesLayout.RowCount = 2;
        diariesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        diariesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        diariesLayout.Size = new System.Drawing.Size(234, 619);
        diariesLayout.TabIndex = 0;
        // 
        // diariesListBox
        // 
        diariesListBox.Dock = DockStyle.Fill;
        diariesListBox.FormattingEnabled = true;
        diariesListBox.IntegralHeight = false;
        diariesListBox.ItemHeight = 20;
        diariesListBox.Location = new System.Drawing.Point(3, 3);
        diariesListBox.Name = "diariesListBox";
        diariesListBox.Size = new System.Drawing.Size(228, 556);
        diariesListBox.TabIndex = 0;
        // 
        // diaryButtonsPanel
        // 
        diaryButtonsPanel.AutoSize = true;
        diaryButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        diaryButtonsPanel.Controls.Add(refreshDiariesButton);
        diaryButtonsPanel.Controls.Add(openDiaryButton);
        diaryButtonsPanel.Controls.Add(createDiaryButton);
        diaryButtonsPanel.Controls.Add(deleteDiaryButton);
        diaryButtonsPanel.Dock = DockStyle.Fill;
        diaryButtonsPanel.FlowDirection = FlowDirection.TopDown;
        diaryButtonsPanel.Location = new System.Drawing.Point(3, 565);
        diaryButtonsPanel.Name = "diaryButtonsPanel";
        diaryButtonsPanel.Size = new System.Drawing.Size(228, 51);
        diaryButtonsPanel.TabIndex = 1;
        diaryButtonsPanel.WrapContents = false;
        // 
        // refreshDiariesButton
        // 
        refreshDiariesButton.AutoSize = true;
        refreshDiariesButton.Location = new System.Drawing.Point(3, 3);
        refreshDiariesButton.Name = "refreshDiariesButton";
        refreshDiariesButton.Size = new System.Drawing.Size(79, 35);
        refreshDiariesButton.TabIndex = 0;
        refreshDiariesButton.Text = "Actualizar";
        refreshDiariesButton.UseVisualStyleBackColor = true;
        // 
        // openDiaryButton
        // 
        openDiaryButton.AutoSize = true;
        openDiaryButton.Location = new System.Drawing.Point(3, 44);
        openDiaryButton.Name = "openDiaryButton";
        openDiaryButton.Size = new System.Drawing.Size(57, 35);
        openDiaryButton.TabIndex = 1;
        openDiaryButton.Text = "Abrir";
        openDiaryButton.UseVisualStyleBackColor = true;
        // 
        // createDiaryButton
        // 
        createDiaryButton.AutoSize = true;
        createDiaryButton.Location = new System.Drawing.Point(3, 85);
        createDiaryButton.Name = "createDiaryButton";
        createDiaryButton.Size = new System.Drawing.Size(68, 35);
        createDiaryButton.TabIndex = 2;
        createDiaryButton.Text = "Crear";
        createDiaryButton.UseVisualStyleBackColor = true;
        // 
        // deleteDiaryButton
        // 
        deleteDiaryButton.AutoSize = true;
        deleteDiaryButton.Location = new System.Drawing.Point(3, 126);
        deleteDiaryButton.Margin = new Padding(3, 3, 3, 0);
        deleteDiaryButton.Name = "deleteDiaryButton";
        deleteDiaryButton.Size = new System.Drawing.Size(71, 35);
        deleteDiaryButton.TabIndex = 3;
        deleteDiaryButton.Text = "Eliminar";
        deleteDiaryButton.UseVisualStyleBackColor = true;
        // 
        // entriesGroupBox
        // 
        entriesGroupBox.Controls.Add(entriesLayout);
        entriesGroupBox.Dock = DockStyle.Fill;
        entriesGroupBox.Location = new System.Drawing.Point(263, 3);
        entriesGroupBox.Name = "entriesGroupBox";
        entriesGroupBox.Padding = new Padding(10);
        entriesGroupBox.Size = new System.Drawing.Size(818, 655);
        entriesGroupBox.TabIndex = 1;
        entriesGroupBox.TabStop = false;
        entriesGroupBox.Text = "Recuerdos";
        // 
        // entriesLayout
        // 
        entriesLayout.ColumnCount = 1;
        entriesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        entriesLayout.Controls.Add(currentDiaryLabel, 0, 0);
        entriesLayout.Controls.Add(entriesListView, 0, 1);
        entriesLayout.Controls.Add(entryEditorLayout, 0, 2);
        entriesLayout.Controls.Add(entryButtonsPanel, 0, 3);
        entriesLayout.Controls.Add(statusLabel, 0, 4);
        entriesLayout.Dock = DockStyle.Fill;
        entriesLayout.Location = new System.Drawing.Point(10, 26);
        entriesLayout.Name = "entriesLayout";
        entriesLayout.RowCount = 5;
        entriesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        entriesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        entriesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        entriesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        entriesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        entriesLayout.Size = new System.Drawing.Size(798, 619);
        entriesLayout.TabIndex = 0;
        // 
        // currentDiaryLabel
        // 
        currentDiaryLabel.AutoSize = true;
        currentDiaryLabel.Dock = DockStyle.Fill;
        currentDiaryLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        currentDiaryLabel.Location = new System.Drawing.Point(3, 0);
        currentDiaryLabel.Name = "currentDiaryLabel";
        currentDiaryLabel.Padding = new Padding(0, 0, 0, 8);
        currentDiaryLabel.Size = new System.Drawing.Size(792, 23);
        currentDiaryLabel.TabIndex = 0;
        currentDiaryLabel.Text = "Selecciona un diario y haz clic en \"Abrir\".";
        // 
        // entriesListView
        // 
        entriesListView.Columns.AddRange(new ColumnHeader[] { columnDate, columnTitle, columnIntensity });
        entriesListView.Dock = DockStyle.Fill;
        entriesListView.FullRowSelect = true;
        entriesListView.HideSelection = false;
        entriesListView.Location = new System.Drawing.Point(3, 26);
        entriesListView.MultiSelect = false;
        entriesListView.Name = "entriesListView";
        entriesListView.Size = new System.Drawing.Size(792, 282);
        entriesListView.TabIndex = 1;
        entriesListView.UseCompatibleStateImageBehavior = false;
        entriesListView.View = View.Details;
        // 
        // columnDate
        // 
        columnDate.Text = "Fecha";
        columnDate.Width = 120;
        // 
        // columnTitle
        // 
        columnTitle.Text = "Título";
        columnTitle.Width = 420;
        // 
        // columnIntensity
        // 
        columnIntensity.Text = "Intensidad";
        columnIntensity.Width = 120;
        // 
        // entryEditorLayout
        // 
        entryEditorLayout.ColumnCount = 2;
        entryEditorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        entryEditorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        entryEditorLayout.Controls.Add(entryDateLabel, 0, 0);
        entryEditorLayout.Controls.Add(entryDatePicker, 1, 0);
        entryEditorLayout.Controls.Add(entryTitleLabel, 0, 1);
        entryEditorLayout.Controls.Add(entryTitleTextBox, 1, 1);
        entryEditorLayout.Controls.Add(entryDescriptionLabel, 0, 2);
        entryEditorLayout.Controls.Add(entryDescriptionTextBox, 1, 2);
        entryEditorLayout.Controls.Add(entryIntensityLabel, 0, 3);
        entryEditorLayout.Controls.Add(entryIntensityNumericUpDown, 1, 3);
        entryEditorLayout.Dock = DockStyle.Fill;
        entryEditorLayout.Location = new System.Drawing.Point(3, 314);
        entryEditorLayout.Name = "entryEditorLayout";
        entryEditorLayout.RowCount = 4;
        entryEditorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        entryEditorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        entryEditorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        entryEditorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        entryEditorLayout.Size = new System.Drawing.Size(792, 282);
        entryEditorLayout.TabIndex = 2;
        // 
        // entryDateLabel
        // 
        entryDateLabel.AutoSize = true;
        entryDateLabel.Dock = DockStyle.Fill;
        entryDateLabel.Location = new System.Drawing.Point(3, 0);
        entryDateLabel.Name = "entryDateLabel";
        entryDateLabel.Padding = new Padding(0, 6, 0, 6);
        entryDateLabel.Size = new System.Drawing.Size(114, 34);
        entryDateLabel.TabIndex = 0;
        entryDateLabel.Text = "Fecha";
        // 
        // entryDatePicker
        // 
        entryDatePicker.Dock = DockStyle.Left;
        entryDatePicker.Format = DateTimePickerFormat.Short;
        entryDatePicker.Location = new System.Drawing.Point(123, 3);
        entryDatePicker.Name = "entryDatePicker";
        entryDatePicker.Size = new System.Drawing.Size(160, 27);
        entryDatePicker.TabIndex = 1;
        // 
        // entryTitleLabel
        // 
        entryTitleLabel.AutoSize = true;
        entryTitleLabel.Dock = DockStyle.Fill;
        entryTitleLabel.Location = new System.Drawing.Point(3, 34);
        entryTitleLabel.Name = "entryTitleLabel";
        entryTitleLabel.Padding = new Padding(0, 6, 0, 6);
        entryTitleLabel.Size = new System.Drawing.Size(114, 34);
        entryTitleLabel.TabIndex = 2;
        entryTitleLabel.Text = "Título";
        // 
        // entryTitleTextBox
        // 
        entryTitleTextBox.Dock = DockStyle.Fill;
        entryTitleTextBox.Location = new System.Drawing.Point(123, 37);
        entryTitleTextBox.Name = "entryTitleTextBox";
        entryTitleTextBox.Size = new System.Drawing.Size(666, 27);
        entryTitleTextBox.TabIndex = 3;
        // 
        // entryDescriptionLabel
        // 
        entryDescriptionLabel.AutoSize = true;
        entryDescriptionLabel.Dock = DockStyle.Fill;
        entryDescriptionLabel.Location = new System.Drawing.Point(3, 68);
        entryDescriptionLabel.Name = "entryDescriptionLabel";
        entryDescriptionLabel.Padding = new Padding(0, 6, 0, 6);
        entryDescriptionLabel.Size = new System.Drawing.Size(114, 196);
        entryDescriptionLabel.TabIndex = 4;
        entryDescriptionLabel.Text = "Descripción";
        // 
        // entryDescriptionTextBox
        // 
        entryDescriptionTextBox.Dock = DockStyle.Fill;
        entryDescriptionTextBox.Location = new System.Drawing.Point(123, 71);
        entryDescriptionTextBox.Multiline = true;
        entryDescriptionTextBox.Name = "entryDescriptionTextBox";
        entryDescriptionTextBox.ScrollBars = ScrollBars.Vertical;
        entryDescriptionTextBox.Size = new System.Drawing.Size(666, 190);
        entryDescriptionTextBox.TabIndex = 5;
        // 
        // entryIntensityLabel
        // 
        entryIntensityLabel.AutoSize = true;
        entryIntensityLabel.Dock = DockStyle.Fill;
        entryIntensityLabel.Location = new System.Drawing.Point(3, 264);
        entryIntensityLabel.Name = "entryIntensityLabel";
        entryIntensityLabel.Padding = new Padding(0, 6, 0, 6);
        entryIntensityLabel.Size = new System.Drawing.Size(114, 48);
        entryIntensityLabel.TabIndex = 6;
        entryIntensityLabel.Text = "Intensidad (1-10)";
        // 
        // entryIntensityNumericUpDown
        // 
        entryIntensityNumericUpDown.Dock = DockStyle.Left;
        entryIntensityNumericUpDown.Location = new System.Drawing.Point(123, 267);
        entryIntensityNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        entryIntensityNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        entryIntensityNumericUpDown.Name = "entryIntensityNumericUpDown";
        entryIntensityNumericUpDown.Size = new System.Drawing.Size(80, 27);
        entryIntensityNumericUpDown.TabIndex = 7;
        entryIntensityNumericUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
        // 
        // entryButtonsPanel
        // 
        entryButtonsPanel.AutoSize = true;
        entryButtonsPanel.Controls.Add(addEntryButton);
        entryButtonsPanel.Controls.Add(updateEntryButton);
        entryButtonsPanel.Controls.Add(deleteEntryButton);
        entryButtonsPanel.Controls.Add(statisticsButton);
        entryButtonsPanel.Dock = DockStyle.Fill;
        entryButtonsPanel.FlowDirection = FlowDirection.RightToLeft;
        entryButtonsPanel.Location = new System.Drawing.Point(3, 602);
        entryButtonsPanel.Name = "entryButtonsPanel";
        entryButtonsPanel.Size = new System.Drawing.Size(792, 35);
        entryButtonsPanel.TabIndex = 3;
        // 
        // addEntryButton
        // 
        addEntryButton.AutoSize = true;
        addEntryButton.Location = new System.Drawing.Point(672, 3);
        addEntryButton.Name = "addEntryButton";
        addEntryButton.Size = new System.Drawing.Size(117, 29);
        addEntryButton.TabIndex = 0;
        addEntryButton.Text = "Añadir recuerdo";
        addEntryButton.UseVisualStyleBackColor = true;
        // 
        // updateEntryButton
        // 
        updateEntryButton.AutoSize = true;
        updateEntryButton.Location = new System.Drawing.Point(536, 3);
        updateEntryButton.Name = "updateEntryButton";
        updateEntryButton.Size = new System.Drawing.Size(130, 29);
        updateEntryButton.TabIndex = 1;
        updateEntryButton.Text = "Actualizar actual";
        updateEntryButton.UseVisualStyleBackColor = true;
        // 
        // deleteEntryButton
        //
        deleteEntryButton.AutoSize = true;
        deleteEntryButton.Location = new System.Drawing.Point(400, 3);
        deleteEntryButton.Name = "deleteEntryButton";
        deleteEntryButton.Size = new System.Drawing.Size(130, 29);
        deleteEntryButton.TabIndex = 2;
        deleteEntryButton.Text = "Eliminar actual";
        deleteEntryButton.UseVisualStyleBackColor = true;
        //
        // statisticsButton
        //
        statisticsButton.AutoSize = true;
        statisticsButton.Enabled = false;
        statisticsButton.Location = new System.Drawing.Point(264, 3);
        statisticsButton.Name = "statisticsButton";
        statisticsButton.Size = new System.Drawing.Size(130, 29);
        statisticsButton.TabIndex = 3;
        statisticsButton.Text = "Estadísticas";
        statisticsButton.UseVisualStyleBackColor = true;
        // 
        // statusLabel
        // 
        statusLabel.AutoSize = true;
        statusLabel.Dock = DockStyle.Fill;
        statusLabel.ForeColor = System.Drawing.Color.DimGray;
        statusLabel.Location = new System.Drawing.Point(3, 640);
        statusLabel.Margin = new Padding(3, 0, 3, 3);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new System.Drawing.Size(792, 16);
        statusLabel.TabIndex = 4;
        statusLabel.Text = "Listo.";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1084, 661);
        Controls.Add(mainLayout);
        MinimumSize = new System.Drawing.Size(900, 600);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Memory Ledger";
        mainLayout.ResumeLayout(false);
        diariesGroupBox.ResumeLayout(false);
        diariesLayout.ResumeLayout(false);
        diariesLayout.PerformLayout();
        diaryButtonsPanel.ResumeLayout(false);
        diaryButtonsPanel.PerformLayout();
        entriesGroupBox.ResumeLayout(false);
        entriesLayout.ResumeLayout(false);
        entriesLayout.PerformLayout();
        entryEditorLayout.ResumeLayout(false);
        entryEditorLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)entryIntensityNumericUpDown).EndInit();
        entryButtonsPanel.ResumeLayout(false);
        entryButtonsPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel mainLayout;
    private GroupBox diariesGroupBox;
    private TableLayoutPanel diariesLayout;
    private ListBox diariesListBox;
    private FlowLayoutPanel diaryButtonsPanel;
    private Button refreshDiariesButton;
    private Button openDiaryButton;
    private Button createDiaryButton;
    private Button deleteDiaryButton;
    private GroupBox entriesGroupBox;
    private TableLayoutPanel entriesLayout;
    private Label currentDiaryLabel;
    private ListView entriesListView;
    private ColumnHeader columnDate;
    private ColumnHeader columnTitle;
    private ColumnHeader columnIntensity;
    private TableLayoutPanel entryEditorLayout;
    private Label entryDateLabel;
    private DateTimePicker entryDatePicker;
    private Label entryTitleLabel;
    private TextBox entryTitleTextBox;
    private Label entryDescriptionLabel;
    private TextBox entryDescriptionTextBox;
    private Label entryIntensityLabel;
    private NumericUpDown entryIntensityNumericUpDown;
    private FlowLayoutPanel entryButtonsPanel;
    private Button addEntryButton;
    private Button updateEntryButton;
    private Button deleteEntryButton;
    private Button statisticsButton;
    private Label statusLabel;
}
