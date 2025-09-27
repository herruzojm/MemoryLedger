using MemoryLedgerApp.Models;

namespace MemoryLedgerApp.Services;

public class DiarySession
{
    private readonly DiaryStorage _storage;
    private readonly string _password;

    public Diary Diary { get; }

    public DiarySession(Diary diary, string password, DiaryStorage storage)
    {
        Diary = diary;
        _password = password;
        _storage = storage;
    }

    public IEnumerable<MemoryEntry> GetEntriesOrderedByDateDesc() =>
        Diary.Entries
            .OrderByDescending(entry => entry.Date)
            .ThenByDescending(entry => entry.Id);

    public MemoryEntry AddEntry(DateTime date, string title, string description, int intensity)
    {
        var entry = new MemoryEntry
        {
            Id = GenerateNextId(),
            Date = date,
            Title = title,
            Description = description,
            Intensity = intensity
        };

        Diary.Entries.Add(entry);
        return entry;
    }

    public bool UpdateEntry(int id, Action<MemoryEntry> updateAction)
    {
        var entry = Diary.Entries.FirstOrDefault(e => e.Id == id);
        if (entry is null)
        {
            return false;
        }

        updateAction(entry);
        return true;
    }

    public bool DeleteEntry(int id)
    {
        var entry = Diary.Entries.FirstOrDefault(e => e.Id == id);
        if (entry is null)
        {
            return false;
        }

        Diary.Entries.Remove(entry);
        return true;
    }

    public IEnumerable<MemoryEntry> SearchEntries(string? title, DateTime? date, int? intensity)
    {
        IEnumerable<MemoryEntry> result = Diary.Entries;

        if (!string.IsNullOrWhiteSpace(title))
        {
            var comparison = StringComparison.OrdinalIgnoreCase;
            result = result.Where(e =>
                e.Title.Contains(title, comparison) ||
                e.Description.Contains(title, comparison));
        }

        if (date.HasValue)
        {
            result = result.Where(e => e.Date.Date == date.Value.Date);
        }

        if (intensity.HasValue)
        {
            result = result.Where(e => e.Intensity == intensity.Value);
        }

        return result.OrderByDescending(e => e.Date).ThenByDescending(e => e.Id);
    }

    public double? AverageIntensity(DateTime start, DateTime end)
    {
        var rangeEntries = Diary.Entries
            .Where(e => e.Date.Date >= start.Date && e.Date.Date <= end.Date)
            .ToList();

        if (rangeEntries.Count == 0)
        {
            return null;
        }

        return rangeEntries.Average(e => e.Intensity);
    }

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _storage.SaveDiaryAsync(Diary, _password, cancellationToken);

    private int GenerateNextId() =>
        Diary.Entries.Count == 0 ? 1 : Diary.Entries.Max(e => e.Id) + 1;
}
