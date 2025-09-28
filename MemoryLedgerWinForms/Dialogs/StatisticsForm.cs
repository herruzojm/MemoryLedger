using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MemoryLedgerApp.Models;

namespace MemoryLedgerWinForms.Dialogs;

public partial class StatisticsForm : Form
{
    private readonly IReadOnlyList<MemoryEntry> _entries;

    public StatisticsForm(IEnumerable<MemoryEntry> entries)
    {
        if (entries is null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        _entries = entries
            .OrderBy(entry => entry.Date.Date)
            .ThenBy(entry => entry.Id)
            .ToList();

        InitializeComponent();
        ConfigureChart();
        InitializeFilters();
        HookEvents();
        UpdateStatistics();
    }

    private void InitializeFilters()
    {
        if (_entries.Count == 0)
        {
            return;
        }

        var first = _entries.First();
        var last = _entries.Last();
        startDatePicker.Value = first.Date.Date;
        endDatePicker.Value = last.Date.Date;
    }

    private void HookEvents()
    {
        startDatePicker.ValueChanged += OnFiltersChanged;
        endDatePicker.ValueChanged += OnFiltersChanged;
    }

    private void OnFiltersChanged(object? sender, EventArgs e)
    {
        UpdateStatistics();
    }

    private void ConfigureChart()
    {
        statisticsChart.Series.Clear();
        statisticsChart.ChartAreas.Clear();
        statisticsChart.Legends.Clear();

        var chartArea = new ChartArea("Main");
        chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        chartArea.AxisX.Title = "Fecha";
        chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
        chartArea.AxisX.LabelStyle.Format = "yyyy-MM-dd";
        chartArea.AxisX.LabelStyle.Angle = -45;
        chartArea.AxisY.Minimum = 0;
        chartArea.AxisY.Maximum = 10;
        chartArea.AxisY.Interval = 1;
        chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
        chartArea.AxisY.Title = "Intensidad";
        statisticsChart.ChartAreas.Add(chartArea);

        var legend = new Legend
        {
            Docking = Docking.Top,
            Alignment = StringAlignment.Center
        };
        statisticsChart.Legends.Add(legend);

        var intensitySeries = new Series("Intensidad")
        {
            ChartType = SeriesChartType.Line,
            XValueType = ChartValueType.Date,
            YValueType = ChartValueType.Double,
            BorderWidth = 2,
            Color = Color.FromArgb(37, 99, 235),
            MarkerStyle = MarkerStyle.Circle,
            MarkerSize = 6,
            MarkerColor = Color.FromArgb(37, 99, 235),
        };
        intensitySeries["LineTension"] = "0.3";
        statisticsChart.Series.Add(intensitySeries);

        var averageSeries = new Series("Promedio")
        {
            ChartType = SeriesChartType.Line,
            XValueType = ChartValueType.Date,
            YValueType = ChartValueType.Double,
            BorderWidth = 2,
            BorderDashStyle = ChartDashStyle.Dash,
            Color = Color.FromArgb(249, 115, 22),
            MarkerStyle = MarkerStyle.None
        };
        statisticsChart.Series.Add(averageSeries);
    }

    private void UpdateStatistics()
    {
        if (_entries.Count == 0)
        {
            summaryLabel.Text = "No hay recuerdos registrados todavía.";
            ShowMessage("Añade recuerdos para ver las estadísticas.");
            return;
        }

        var start = startDatePicker.Checked ? startDatePicker.Value.Date : (DateTime?)null;
        var end = endDatePicker.Checked ? endDatePicker.Value.Date : (DateTime?)null;

        if (start.HasValue && end.HasValue && start.Value > end.Value)
        {
            summaryLabel.Text = "La fecha inicial no puede ser posterior a la final.";
            ShowMessage("Selecciona un periodo válido (la fecha inicial no puede ser posterior a la final).");
            return;
        }

        var filtered = _entries
            .Where(entry => (!start.HasValue || entry.Date.Date >= start.Value) && (!end.HasValue || entry.Date.Date <= end.Value))
            .OrderBy(entry => entry.Date)
            .ThenBy(entry => entry.Id)
            .ToList();

        if (filtered.Count == 0)
        {
            summaryLabel.Text = "No hay recuerdos en el rango seleccionado.";
            ShowMessage("No hay recuerdos en el rango seleccionado.");
            return;
        }

        var average = filtered.Average(entry => entry.Intensity);
        summaryLabel.Text = $"Intensidad promedio {DescribeRange(start, end)}: {average:F2}";

        ShowChart();

        var intensitySeries = statisticsChart.Series["Intensidad"];
        var averageSeries = statisticsChart.Series["Promedio"];
        intensitySeries.Points.Clear();
        averageSeries.Points.Clear();

        foreach (var entry in filtered)
        {
            var date = entry.Date.Date;
            intensitySeries.Points.AddXY(date, entry.Intensity);
            averageSeries.Points.AddXY(date, average);
        }
    }

    private static string DescribeRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue)
        {
            return $"del {start.Value:yyyy-MM-dd} al {end.Value:yyyy-MM-dd}";
        }

        if (start.HasValue)
        {
            return $"desde el {start.Value:yyyy-MM-dd}";
        }

        if (end.HasValue)
        {
            return $"hasta el {end.Value:yyyy-MM-dd}";
        }

        return "para todos los recuerdos";
    }

    private void ShowMessage(string message)
    {
        chartPanel.Visible = false;
        emptyLabel.Visible = true;
        emptyLabel.Text = message;
    }

    private void ShowChart()
    {
        chartPanel.Visible = true;
        emptyLabel.Visible = false;
        emptyLabel.Text = string.Empty;
    }
}
