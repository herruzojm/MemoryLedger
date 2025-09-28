using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MemoryLedgerApp.Models;
using MemoryLedgerApp.Services;
using MemoryLedgerWinForms.Dialogs;

namespace MemoryLedgerWinForms;

public partial class MainForm : Form
{
    private readonly DiaryStorage _storage;
    private DiarySession? _currentSession;

    public MainForm(DiaryStorage storage)
    {
        _storage = storage;
        InitializeComponent();
        HookEvents();
        UpdateDiaryButtons();
        UpdateEntryControls();
        LoadDiaries();
    }

    private void HookEvents()
    {
        refreshDiariesButton.Click += (_, _) => LoadDiaries();
        createDiaryButton.Click += async (_, _) => await CreateDiaryAsync();
        openDiaryButton.Click += async (_, _) => await OpenSelectedDiaryAsync();
        deleteDiaryButton.Click += async (_, _) => await DeleteSelectedDiaryAsync();
        diariesListBox.DoubleClick += async (_, _) => await OpenSelectedDiaryAsync();
        diariesListBox.SelectedIndexChanged += (_, _) => UpdateDiaryButtons();

        entriesListView.SelectedIndexChanged += (_, _) => OnEntrySelectionChanged();
        entriesListView.DoubleClick += (_, _) => LoadSelectedEntryIntoEditor();
        addEntryButton.Click += async (_, _) => await AddEntryAsync();
        updateEntryButton.Click += async (_, _) => await UpdateEntryAsync();
        deleteEntryButton.Click += async (_, _) => await DeleteEntryAsync();
        statisticsButton.Click += (_, _) => ShowStatistics();
    }

    private void LoadDiaries()
    {
        var previousSelection = diariesListBox.SelectedItem?.ToString() ?? _currentSession?.Diary.Name;
        var diaries = _storage.ListDiaries().ToList();
        diariesListBox.BeginUpdate();
        diariesListBox.Items.Clear();
        diariesListBox.Items.AddRange(diaries.Cast<object>().ToArray());
        diariesListBox.EndUpdate();

        if (!string.IsNullOrWhiteSpace(previousSelection))
        {
            SelectDiary(previousSelection);
        }

        UpdateDiaryButtons();
        SetStatus(diaries.Count == 0 ? "No hay diarios disponibles." : $"{diaries.Count} diario(s) disponibles.");
    }

    private async Task CreateDiaryAsync()
    {
        var result = DialogService.PromptForDiaryCreation(this);
        if (result is null)
        {
            return;
        }

        var (name, password) = result.Value;
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show(this, "El nombre y la contraseña son obligatorios.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            await _storage.CreateDiaryAsync(name.Trim(), password.Trim());
            LoadDiaries();
            SelectDiary(name);
            SetStatus($"Diario '{name}' creado correctamente.");
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(this, ex.Message, "No se pudo crear", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task OpenSelectedDiaryAsync()
    {
        if (diariesListBox.SelectedItem is not string diaryName)
        {
            MessageBox.Show(this, "Selecciona un diario de la lista.", "Ningún diario seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var password = DialogService.PromptForPassword(this, diaryName);
        if (password is null)
        {
            return;
        }

        try
        {
            var diary = await _storage.LoadDiaryAsync(diaryName, password);
            _currentSession = new DiarySession(diary, password, _storage);
            currentDiaryLabel.Text = $"Diario abierto: {diaryName}";
            PopulateEntries();
            ClearEntryEditor();
            UpdateEntryControls();
            SetStatus($"Diario '{diaryName}' abierto correctamente.");
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show(this, "Contraseña incorrecta.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (FileNotFoundException)
        {
            MessageBox.Show(this, "El diario ya no existe.", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadDiaries();
        }
    }

    private async Task DeleteSelectedDiaryAsync()
    {
        if (diariesListBox.SelectedItem is not string diaryName)
        {
            MessageBox.Show(this, "Selecciona un diario de la lista.", "Ningún diario seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var password = DialogService.PromptForPassword(this, diaryName);
        if (password is null)
        {
            return;
        }

        var confirm = MessageBox.Show(this, $"¿Seguro que deseas eliminar el diario '{diaryName}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            // Validar la contraseña antes de eliminar.
            await _storage.LoadDiaryAsync(diaryName, password);
            _storage.DeleteDiary(diaryName);
            if (_currentSession?.Diary.Name == diaryName)
            {
                CloseCurrentSession();
            }

            LoadDiaries();
            SetStatus($"Diario '{diaryName}' eliminado correctamente.");
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show(this, "Contraseña incorrecta.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (FileNotFoundException)
        {
            MessageBox.Show(this, "El diario ya no existe.", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadDiaries();
        }
    }

    private void PopulateEntries()
    {
        entriesListView.BeginUpdate();
        entriesListView.Items.Clear();

        if (_currentSession is not null)
        {
            foreach (var entry in _currentSession.GetEntriesOrderedByDateDesc())
            {
                var item = new ListViewItem(entry.Date.ToString("yyyy-MM-dd"))
                {
                    Tag = entry
                };
                item.SubItems.Add(entry.Title);
                item.SubItems.Add(entry.Intensity.ToString());
                entriesListView.Items.Add(item);
            }
        }

        entriesListView.EndUpdate();
        UpdateEntryControls();
    }

    private async Task AddEntryAsync()
    {
        if (!EnsureSessionActive())
        {
            return;
        }

        if (!TryGetEntryInput(out var date, out var title, out var description, out var intensity))
        {
            return;
        }

        var entry = _currentSession!.AddEntry(date, title, description, intensity);
        await _currentSession.SaveAsync();
        PopulateEntries();
        SelectEntry(entry);
        SetStatus("Recuerdo añadido correctamente.");
    }

    private async Task UpdateEntryAsync()
    {
        if (!EnsureSessionActive())
        {
            return;
        }

        if (GetSelectedEntry() is not MemoryEntry entry)
        {
            MessageBox.Show(this, "Selecciona el recuerdo que deseas actualizar.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!TryGetEntryInput(out var date, out var title, out var description, out var intensity))
        {
            return;
        }

        _currentSession!.UpdateEntry(entry.Id, e =>
        {
            e.Date = date;
            e.Title = title;
            e.Description = description;
            e.Intensity = intensity;
        });

        await _currentSession.SaveAsync();
        PopulateEntries();
        SelectEntry(entry);
        SetStatus("Recuerdo actualizado correctamente.");
    }

    private async Task DeleteEntryAsync()
    {
        if (!EnsureSessionActive())
        {
            return;
        }

        if (GetSelectedEntry() is not MemoryEntry entry)
        {
            MessageBox.Show(this, "Selecciona el recuerdo que deseas eliminar.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(this, "¿Seguro que deseas eliminar este recuerdo?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        if (_currentSession!.DeleteEntry(entry.Id))
        {
            await _currentSession.SaveAsync();
            PopulateEntries();
            ClearEntryEditor();
            SetStatus("Recuerdo eliminado correctamente.");
        }
    }

    private void OnEntrySelectionChanged()
    {
        LoadSelectedEntryIntoEditor();
        UpdateEntryControls();
    }

    private void LoadSelectedEntryIntoEditor()
    {
        if (GetSelectedEntry() is MemoryEntry entry)
        {
            entryDatePicker.Value = entry.Date;
            entryTitleTextBox.Text = entry.Title;
            entryDescriptionTextBox.Text = entry.Description;
            entryIntensityNumericUpDown.Value = Math.Clamp(entry.Intensity, (int)entryIntensityNumericUpDown.Minimum, (int)entryIntensityNumericUpDown.Maximum);
        }
    }

    private void ClearEntryEditor()
    {
        entryDatePicker.Value = DateTime.Today;
        entryTitleTextBox.Clear();
        entryDescriptionTextBox.Clear();
        entryIntensityNumericUpDown.Value = 5;
    }

    private void SelectDiary(string diaryName)
    {
        for (var i = 0; i < diariesListBox.Items.Count; i++)
        {
            if (string.Equals(diaryName, diariesListBox.Items[i]?.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                diariesListBox.SelectedIndex = i;
                break;
            }
        }
    }

    private void SelectEntry(MemoryEntry entry)
    {
        foreach (ListViewItem item in entriesListView.Items)
        {
            if (item.Tag is MemoryEntry current && current.Id == entry.Id)
            {
                item.Selected = true;
                item.EnsureVisible();
                break;
            }
        }
    }

    private MemoryEntry? GetSelectedEntry() => entriesListView.SelectedItems.Count == 0
        ? null
        : entriesListView.SelectedItems[0].Tag as MemoryEntry;

    private bool TryGetEntryInput(out DateTime date, out string title, out string description, out int intensity)
    {
        date = entryDatePicker.Value.Date;
        title = entryTitleTextBox.Text.Trim();
        description = entryDescriptionTextBox.Text.Trim();
        intensity = (int)entryIntensityNumericUpDown.Value;

        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show(this, "El título es obligatorio.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            entryTitleTextBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            MessageBox.Show(this, "La descripción es obligatoria.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            entryDescriptionTextBox.Focus();
            return false;
        }

        return true;
    }

    private bool EnsureSessionActive()
    {
        if (_currentSession is null)
        {
            MessageBox.Show(this, "Abre un diario antes de gestionar recuerdos.", "Ningún diario abierto", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        return true;
    }

    private void CloseCurrentSession()
    {
        _currentSession = null;
        currentDiaryLabel.Text = "Selecciona un diario y haz clic en \"Abrir\".";
        entriesListView.Items.Clear();
        ClearEntryEditor();
        UpdateEntryControls();
    }

    private void UpdateDiaryButtons()
    {
        var hasSelection = diariesListBox.SelectedItem is string;
        openDiaryButton.Enabled = hasSelection;
        deleteDiaryButton.Enabled = hasSelection;
    }

    private void UpdateEntryControls()
    {
        var hasSession = _currentSession is not null;
        var hasSelection = GetSelectedEntry() is not null;

        entryDatePicker.Enabled = hasSession;
        entryTitleTextBox.Enabled = hasSession;
        entryDescriptionTextBox.Enabled = hasSession;
        entryIntensityNumericUpDown.Enabled = hasSession;
        addEntryButton.Enabled = hasSession;
        updateEntryButton.Enabled = hasSession && hasSelection;
        deleteEntryButton.Enabled = hasSession && hasSelection;
        var hasEntries = hasSession && _currentSession!.Diary.Entries.Count > 0;
        statisticsButton.Enabled = hasEntries;
    }

    private void ShowStatistics()
    {
        if (!EnsureSessionActive())
        {
            return;
        }

        if (_currentSession!.Diary.Entries.Count == 0)
        {
            MessageBox.Show(this, "No hay recuerdos registrados todavía.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new StatisticsForm(_currentSession.Diary.Entries);
        dialog.ShowDialog(this);
    }

    private void SetStatus(string message)
    {
        statusLabel.Text = message;
    }
}
