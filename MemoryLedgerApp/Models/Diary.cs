namespace MemoryLedgerApp.Models;

public class Diary
{
    public string Name { get; set; } = string.Empty;
    public List<MemoryEntry> Entries { get; set; } = new();
}
