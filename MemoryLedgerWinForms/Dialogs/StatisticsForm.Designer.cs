using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MemoryLedgerWinForms.Dialogs;

partial class StatisticsForm
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
        mainLayout = new TableLayoutPanel();
        filterGroupBox = new GroupBox();
        filterLayout = new TableLayoutPanel();
        startLabel = new Label();
        startDatePicker = new DateTimePicker();
        endLabel = new Label();
        endDatePicker = new DateTimePicker();
        summaryLabel = new Label();
        chartPanel = new Panel();
        statisticsChart = new Chart();
        emptyLabel = new Label();
        buttonsPanel = new FlowLayoutPanel();
        closeButton = new Button();
        mainLayout.SuspendLayout();
        filterGroupBox.SuspendLayout();
        filterLayout.SuspendLayout();
        chartPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)statisticsChart).BeginInit();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        //
        // mainLayout
        //
        mainLayout.ColumnCount = 1;
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayout.Controls.Add(filterGroupBox, 0, 0);
        mainLayout.Controls.Add(summaryLabel, 0, 1);
        mainLayout.Controls.Add(chartPanel, 0, 2);
        mainLayout.Controls.Add(emptyLabel, 0, 3);
        mainLayout.Controls.Add(buttonsPanel, 0, 4);
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.Location = new System.Drawing.Point(10, 10);
        mainLayout.Name = "mainLayout";
        mainLayout.RowCount = 5;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.Size = new System.Drawing.Size(664, 441);
        mainLayout.TabIndex = 0;
        //
        // filterGroupBox
        //
        filterGroupBox.Controls.Add(filterLayout);
        filterGroupBox.Dock = DockStyle.Fill;
        filterGroupBox.Location = new System.Drawing.Point(3, 3);
        filterGroupBox.Name = "filterGroupBox";
        filterGroupBox.Padding = new Padding(10);
        filterGroupBox.Size = new System.Drawing.Size(658, 94);
        filterGroupBox.TabIndex = 0;
        filterGroupBox.TabStop = false;
        filterGroupBox.Text = "Filtrar por periodo";
        //
        // filterLayout
        //
        filterLayout.ColumnCount = 2;
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        filterLayout.Controls.Add(startLabel, 0, 0);
        filterLayout.Controls.Add(startDatePicker, 0, 1);
        filterLayout.Controls.Add(endLabel, 1, 0);
        filterLayout.Controls.Add(endDatePicker, 1, 1);
        filterLayout.Dock = DockStyle.Fill;
        filterLayout.Location = new System.Drawing.Point(10, 30);
        filterLayout.Name = "filterLayout";
        filterLayout.RowCount = 2;
        filterLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        filterLayout.Size = new System.Drawing.Size(638, 54);
        filterLayout.TabIndex = 0;
        //
        // startLabel
        //
        startLabel.AutoSize = true;
        startLabel.Dock = DockStyle.Fill;
        startLabel.Location = new System.Drawing.Point(3, 0);
        startLabel.Name = "startLabel";
        startLabel.Padding = new Padding(0, 0, 0, 6);
        startLabel.Size = new System.Drawing.Size(313, 20);
        startLabel.TabIndex = 0;
        startLabel.Text = "Fecha inicial";
        //
        // startDatePicker
        //
        startDatePicker.Checked = false;
        startDatePicker.Dock = DockStyle.Left;
        startDatePicker.Format = DateTimePickerFormat.Short;
        startDatePicker.Location = new System.Drawing.Point(3, 23);
        startDatePicker.Name = "startDatePicker";
        startDatePicker.ShowCheckBox = true;
        startDatePicker.Size = new System.Drawing.Size(160, 27);
        startDatePicker.TabIndex = 1;
        //
        // endLabel
        //
        endLabel.AutoSize = true;
        endLabel.Dock = DockStyle.Fill;
        endLabel.Location = new System.Drawing.Point(322, 0);
        endLabel.Name = "endLabel";
        endLabel.Padding = new Padding(0, 0, 0, 6);
        endLabel.Size = new System.Drawing.Size(313, 20);
        endLabel.TabIndex = 2;
        endLabel.Text = "Fecha final";
        //
        // endDatePicker
        //
        endDatePicker.Checked = false;
        endDatePicker.Dock = DockStyle.Left;
        endDatePicker.Format = DateTimePickerFormat.Short;
        endDatePicker.Location = new System.Drawing.Point(322, 23);
        endDatePicker.Name = "endDatePicker";
        endDatePicker.ShowCheckBox = true;
        endDatePicker.Size = new System.Drawing.Size(160, 27);
        endDatePicker.TabIndex = 3;
        //
        // summaryLabel
        //
        summaryLabel.AutoSize = true;
        summaryLabel.Dock = DockStyle.Fill;
        summaryLabel.ForeColor = System.Drawing.Color.DimGray;
        summaryLabel.Location = new System.Drawing.Point(3, 100);
        summaryLabel.Name = "summaryLabel";
        summaryLabel.Padding = new Padding(0, 6, 0, 6);
        summaryLabel.Size = new System.Drawing.Size(658, 32);
        summaryLabel.TabIndex = 1;
        summaryLabel.Text = "Selecciona un rango para ver el promedio.";
        //
        // chartPanel
        //
        chartPanel.Controls.Add(statisticsChart);
        chartPanel.Dock = DockStyle.Fill;
        chartPanel.Location = new System.Drawing.Point(3, 135);
        chartPanel.Name = "chartPanel";
        chartPanel.Size = new System.Drawing.Size(658, 236);
        chartPanel.TabIndex = 2;
        //
        // statisticsChart
        //
        statisticsChart.Dock = DockStyle.Fill;
        statisticsChart.Location = new System.Drawing.Point(0, 0);
        statisticsChart.Name = "statisticsChart";
        statisticsChart.Size = new System.Drawing.Size(658, 236);
        statisticsChart.TabIndex = 0;
        statisticsChart.Text = "chart1";
        //
        // emptyLabel
        //
        emptyLabel.AutoSize = true;
        emptyLabel.Dock = DockStyle.Fill;
        emptyLabel.ForeColor = System.Drawing.Color.DimGray;
        emptyLabel.Location = new System.Drawing.Point(3, 374);
        emptyLabel.Name = "emptyLabel";
        emptyLabel.Padding = new Padding(0, 10, 0, 10);
        emptyLabel.Size = new System.Drawing.Size(658, 40);
        emptyLabel.TabIndex = 3;
        emptyLabel.Text = "";
        emptyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        emptyLabel.Visible = false;
        //
        // buttonsPanel
        //
        buttonsPanel.AutoSize = true;
        buttonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        buttonsPanel.Controls.Add(closeButton);
        buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Location = new System.Drawing.Point(3, 417);
        buttonsPanel.Name = "buttonsPanel";
        buttonsPanel.Size = new System.Drawing.Size(658, 21);
        buttonsPanel.TabIndex = 4;
        buttonsPanel.WrapContents = false;
        //
        // closeButton
        //
        closeButton.AutoSize = true;
        closeButton.DialogResult = DialogResult.OK;
        closeButton.Location = new System.Drawing.Point(565, 3);
        closeButton.Name = "closeButton";
        closeButton.Size = new System.Drawing.Size(90, 35);
        closeButton.TabIndex = 0;
        closeButton.Text = "Cerrar";
        closeButton.UseVisualStyleBackColor = true;
        //
        // StatisticsForm
        //
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        ClientSize = new System.Drawing.Size(684, 461);
        Controls.Add(mainLayout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "StatisticsForm";
        Padding = new Padding(10);
        ShowIcon = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Estadísticas del diario";
        AcceptButton = closeButton;
        mainLayout.ResumeLayout(false);
        mainLayout.PerformLayout();
        filterGroupBox.ResumeLayout(false);
        filterLayout.ResumeLayout(false);
        filterLayout.PerformLayout();
        chartPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)statisticsChart).EndInit();
        buttonsPanel.ResumeLayout(false);
        buttonsPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel mainLayout;
    private GroupBox filterGroupBox;
    private TableLayoutPanel filterLayout;
    private Label startLabel;
    private DateTimePicker startDatePicker;
    private Label endLabel;
    private DateTimePicker endDatePicker;
    private Label summaryLabel;
    private Panel chartPanel;
    private Chart statisticsChart;
    private Label emptyLabel;
    private FlowLayoutPanel buttonsPanel;
    private Button closeButton;
}
