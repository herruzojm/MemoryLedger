namespace MemoryLedgerApp.Models;

public class MemoryEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Intensity { get; set; }
}
