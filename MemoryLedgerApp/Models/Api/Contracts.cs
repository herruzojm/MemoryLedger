namespace MemoryLedgerApp.Models.Api;

public record ApiResponse(bool Success, string Message);

public record ApiResponse<T>(bool Success, string Message, T? Data);

public record CreateDiaryRequest(string Name, string Password);

public record DiaryCredentials(string Name, string Password);

public record DeleteDiaryRequest(string Name, string Password);

public record AddEntryRequest(string DiaryName, string Password, DateTime Date, string Title, string Description, int Intensity);

public record UpdateEntryRequest(string DiaryName, string Password, int EntryId, DateTime Date, string Title, string Description, int Intensity);

public record DeleteEntryRequest(string DiaryName, string Password, int EntryId);

public record SearchEntriesRequest(string DiaryName, string Password, string? Text, DateTime? Date, int? Intensity);

public record AverageIntensityRequest(string DiaryName, string Password, DateTime? Start, DateTime? End);

public record MemoryEntryDto(int Id, DateTime Date, string Title, string Description, int Intensity);

public record DiaryDetailDto(string Name, IReadOnlyList<MemoryEntryDto> Entries);

public record AverageIntensityResponse(double? Average, DateTime Start, DateTime End);
