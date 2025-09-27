using MemoryLedgerApp.Models;
using MemoryLedgerApp.Models.Api;
using MemoryLedgerApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new DiaryStorage(AppContext.BaseDirectory));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/diaries", (DiaryStorage storage) =>
{
    var diaries = storage.ListDiaries().ToList();
    var message = diaries.Count switch
    {
        0 => "No diaries available yet.",
        1 => "1 diary available.",
        _ => $"{diaries.Count} diaries available."
    };

    return Results.Json(new ApiResponse<IReadOnlyList<string>>(true, message, diaries));
});

app.MapPost("/api/diaries/create", async (DiaryStorage storage, CreateDiaryRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.Json(new ApiResponse(false, "Name and password are required."), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        await storage.CreateDiaryAsync(request.Name.Trim(), request.Password.Trim());
        return Results.Json(new ApiResponse(true, "Diary created successfully."));
    }
    catch (InvalidOperationException ex)
    {
        return Results.Json(new ApiResponse(false, ex.Message), statusCode: StatusCodes.Status409Conflict);
    }
});

app.MapPost("/api/diaries/open", async (DiaryStorage storage, DiaryCredentials request) =>
{
    if (HasMissing(request.Name, request.Password))
    {
        return Results.Json(new ApiResponse<DiaryDetailDto>(false, "Name and password are required.", null), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        var session = await CreateSessionAsync(storage, request.Name, request.Password);
        var detail = new DiaryDetailDto(
            session.Diary.Name,
            session.GetEntriesOrderedByDateDesc().Select(ToDto).ToList());

        return Results.Json(new ApiResponse<DiaryDetailDto>(true, "Diary opened successfully.", detail));
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Json(new ApiResponse<DiaryDetailDto>(false, "Incorrect password.", null), statusCode: StatusCodes.Status401Unauthorized);
    }
    catch (FileNotFoundException)
    {
        return Results.Json(new ApiResponse<DiaryDetailDto>(false, "Diary not found.", null), statusCode: StatusCodes.Status404NotFound);
    }
});

app.MapPost("/api/diaries/delete", async (DiaryStorage storage, DeleteDiaryRequest request) =>
{
    if (HasMissing(request.Name, request.Password))
    {
        return Results.Json(new ApiResponse(false, "Name and password are required."), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        // Validate password before deletion.
        await storage.LoadDiaryAsync(request.Name, request.Password);
        storage.DeleteDiary(request.Name);
        return Results.Json(new ApiResponse(true, "Diary deleted successfully."));
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Json(new ApiResponse(false, "Incorrect password."), statusCode: StatusCodes.Status401Unauthorized);
    }
    catch (FileNotFoundException)
    {
        return Results.Json(new ApiResponse(false, "Diary not found."), statusCode: StatusCodes.Status404NotFound);
    }
});

app.MapPost("/api/entries/add", async (DiaryStorage storage, AddEntryRequest request) =>
{
    var validation = ValidateEntryRequest(request.DiaryName, request.Password, request.Title, request.Description, request.Intensity);
    if (validation is not null)
    {
        return validation;
    }

    try
    {
        var session = await CreateSessionAsync(storage, request.DiaryName, request.Password);
        session.AddEntry(request.Date, request.Title.Trim(), request.Description.Trim(), request.Intensity);
        await session.SaveAsync();
        return Results.Json(new ApiResponse(true, "Memory added successfully."));
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Json(new ApiResponse(false, "Incorrect password."), statusCode: StatusCodes.Status401Unauthorized);
    }
    catch (FileNotFoundException)
    {
        return Results.Json(new ApiResponse(false, "Diary not found."), statusCode: StatusCodes.Status404NotFound);
    }
});

app.MapPost("/api/entries/update", async (DiaryStorage storage, UpdateEntryRequest request) =>
{
    var validation = ValidateEntryRequest(request.DiaryName, request.Password, request.Title, request.Description, request.Intensity);
    if (validation is not null)
    {
        return validation;
    }

    try
    {
        var session = await CreateSessionAsync(storage, request.DiaryName, request.Password);
        var updated = session.UpdateEntry(request.EntryId, entry =>
        {
            entry.Date = request.Date;
            entry.Title = request.Title.Trim();
            entry.Description = request.Description.Trim();
            entry.Intensity = request.Intensity;
        });

        if (!updated)
        {
            return Results.Json(new ApiResponse(false, "Memory not found."), statusCode: StatusCodes.Status404NotFound);
        }

        await session.SaveAsync();
        return Results.Json(new ApiResponse(true, "Memory updated successfully."));
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Json(new ApiResponse(false, "Incorrect password."), statusCode: StatusCodes.Status401Unauthorized);
    }
    catch (FileNotFoundException)
    {
        return Results.Json(new ApiResponse(false, "Diary not found."), statusCode: StatusCodes.Status404NotFound);
    }
});

app.MapPost("/api/entries/delete", async (DiaryStorage storage, DeleteEntryRequest request) =>
{
    if (HasMissing(request.DiaryName, request.Password))
    {
        return Results.Json(new ApiResponse(false, "Name and password are required."), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        var session = await CreateSessionAsync(storage, request.DiaryName, request.Password);
        var deleted = session.DeleteEntry(request.EntryId);

        if (!deleted)
        {
            return Results.Json(new ApiResponse(false, "Memory not found."), statusCode: StatusCodes.Status404NotFound);
        }

        await session.SaveAsync();
        return Results.Json(new ApiResponse(true, "Memory deleted successfully."));
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Json(new ApiResponse(false, "Incorrect password."), statusCode: StatusCodes.Status401Unauthorized);
    }
    catch (FileNotFoundException)
    {
        return Results.Json(new ApiResponse(false, "Diary not found."), statusCode: StatusCodes.Status404NotFound);
    }
});

app.MapPost("/api/entries/search", async (DiaryStorage storage, SearchEntriesRequest request) =>
{
    if (HasMissing(request.DiaryName, request.Password))
    {
        return Results.Json(new ApiResponse<IEnumerable<MemoryEntryDto>>(false, "Name and password are required.", null), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        var session = await CreateSessionAsync(storage, request.DiaryName, request.Password);
        var results = session
            .SearchEntries(request.Text, request.Date, request.Intensity)
            .Select(ToDto)
            .ToList();

        var message = results.Count == 0 ? "No memories matched your filters." : $"Found {results.Count} memories.";
        return Results.Json(new ApiResponse<IReadOnlyList<MemoryEntryDto>>(true, message, results));
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Json(new ApiResponse<IEnumerable<MemoryEntryDto>>(false, "Incorrect password.", null), statusCode: StatusCodes.Status401Unauthorized);
    }
    catch (FileNotFoundException)
    {
        return Results.Json(new ApiResponse<IEnumerable<MemoryEntryDto>>(false, "Diary not found.", null), statusCode: StatusCodes.Status404NotFound);
    }
});

app.MapPost("/api/entries/average", async (DiaryStorage storage, AverageIntensityRequest request) =>
{
    if (HasMissing(request.DiaryName, request.Password))
    {
        return Results.Json(new ApiResponse<AverageIntensityResponse>(false, "Name and password are required.", null), statusCode: StatusCodes.Status400BadRequest);
    }

    if (request.Start.HasValue && request.End.HasValue && request.End < request.Start)
    {
        return Results.Json(new ApiResponse<AverageIntensityResponse>(false, "End date cannot be before the start date.", null), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        var session = await CreateSessionAsync(storage, request.DiaryName, request.Password);
        var start = request.Start ?? DateTime.Today.AddYears(-1);
        var end = request.End ?? DateTime.Today;
        var average = session.AverageIntensity(start, end);
        var response = new AverageIntensityResponse(average, start, end);
        var message = average.HasValue
            ? $"Average intensity from {start:yyyy-MM-dd} to {end:yyyy-MM-dd} is {average:0.00}."
            : "No memories were found in the selected range.";

        return Results.Json(new ApiResponse<AverageIntensityResponse>(true, message, response));
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Json(new ApiResponse<AverageIntensityResponse>(false, "Incorrect password.", null), statusCode: StatusCodes.Status401Unauthorized);
    }
    catch (FileNotFoundException)
    {
        return Results.Json(new ApiResponse<AverageIntensityResponse>(false, "Diary not found.", null), statusCode: StatusCodes.Status404NotFound);
    }
});

app.MapFallbackToFile("index.html");

app.Run();

static bool HasMissing(string? name, string? password) =>
    string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password);

static IResult? ValidateEntryRequest(string diaryName, string password, string title, string description, int intensity)
{
    if (HasMissing(diaryName, password))
    {
        return Results.Json(new ApiResponse(false, "Name and password are required."), statusCode: StatusCodes.Status400BadRequest);
    }

    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
    {
        return Results.Json(new ApiResponse(false, "Title and description are required."), statusCode: StatusCodes.Status400BadRequest);
    }

    if (intensity is < 0 or > 10)
    {
        return Results.Json(new ApiResponse(false, "Intensity must be between 0 and 10."), statusCode: StatusCodes.Status400BadRequest);
    }

    return null;
}

static async Task<DiarySession> CreateSessionAsync(DiaryStorage storage, string name, string password)
{
    var diary = await storage.LoadDiaryAsync(name, password);
    return new DiarySession(diary, password, storage);
}

static MemoryEntryDto ToDto(MemoryEntry entry) =>
    new(entry.Id, entry.Date, entry.Title, entry.Description, entry.Intensity);
