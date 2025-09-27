using MemoryLedgerApp.Models;
using MemoryLedgerApp.Services;
using MemoryLedgerApp.Utilities;

var storage = new DiaryStorage(AppContext.BaseDirectory);
await ShowMainMenuAsync(storage);

static async Task ShowMainMenuAsync(DiaryStorage storage)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("============================");
        Console.WriteLine("      MemoryLedger");
        Console.WriteLine("============================\n");
        Console.WriteLine("1. Crear un diario");
        Console.WriteLine("2. Abrir un diario");
        Console.WriteLine("3. Borrar un diario");
        Console.WriteLine("0. Salir\n");

        var option = InputHelper.Prompt("Selecciona una opción: ");

        switch (option)
        {
            case "1":
                await CreateDiaryAsync(storage);
                break;
            case "2":
                await OpenDiaryAsync(storage);
                break;
            case "3":
                DeleteDiary(storage);
                break;
            case "0":
                Console.WriteLine("Hasta luego!");
                return;
            default:
                Console.WriteLine("Opción no válida.");
                Pause();
                break;
        }
    }
}

static async Task CreateDiaryAsync(DiaryStorage storage)
{
    Console.Clear();
    Console.WriteLine("=== Crear un nuevo diario ===\n");

    var name = InputHelper.PromptRequired("Nombre del diario: ");
    var password = InputHelper.PromptRequired("Clave del diario: ");

    try
    {
        await storage.CreateDiaryAsync(name, password);
        Console.WriteLine("Diario creado correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"No se pudo crear el diario: {ex.Message}");
    }

    Pause();
}

static async Task OpenDiaryAsync(DiaryStorage storage)
{
    Console.Clear();
    Console.WriteLine("=== Abrir un diario ===\n");

    var diaries = storage.ListDiaries().ToList();
    if (!diaries.Any())
    {
        Console.WriteLine("No hay diarios disponibles. Crea uno primero.");
        Pause();
        return;
    }

    Console.WriteLine("Diarios disponibles:");
    foreach (var diaryName in diaries)
    {
        Console.WriteLine($"- {diaryName}");
    }

    var name = InputHelper.PromptRequired("Nombre del diario a abrir: ");
    var password = InputHelper.PromptRequired("Clave del diario: ");

    try
    {
        var diary = await storage.LoadDiaryAsync(name, password);
        var session = new DiarySession(diary, password, storage);
        await RunDiarySessionAsync(session);
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine("Clave incorrecta. No se pudo abrir el diario.");
        Pause();
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("El diario no existe.");
        Pause();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"No se pudo abrir el diario: {ex.Message}");
        Pause();
    }
}

static void DeleteDiary(DiaryStorage storage)
{
    Console.Clear();
    Console.WriteLine("=== Borrar un diario ===\n");

    var diaries = storage.ListDiaries().ToList();
    if (!diaries.Any())
    {
        Console.WriteLine("No hay diarios para borrar.");
        Pause();
        return;
    }

    Console.WriteLine("Diarios disponibles:");
    foreach (var diaryName in diaries)
    {
        Console.WriteLine($"- {diaryName}");
    }

    var name = InputHelper.PromptRequired("Nombre del diario a borrar: ");
    var password = InputHelper.PromptRequired("Confirma la clave del diario: ");

    try
    {
        // Intentamos abrirlo para validar la clave antes de borrarlo
        storage.LoadDiaryAsync(name, password).GetAwaiter().GetResult();
        storage.DeleteDiary(name);
        Console.WriteLine("Diario borrado correctamente.");
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine("Clave incorrecta. No se borró el diario.");
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("El diario no existe.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"No se pudo borrar el diario: {ex.Message}");
    }

    Pause();
}

static async Task RunDiarySessionAsync(DiarySession session)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine($"=== Diario: {session.Diary.Name} ===\n");
        DisplayEntries(session.GetEntriesOrderedByDateDesc());

        Console.WriteLine();
        Console.WriteLine("1. Añadir recuerdo");
        Console.WriteLine("2. Buscar recuerdos");
        Console.WriteLine("3. Editar recuerdo");
        Console.WriteLine("4. Borrar recuerdo");
        Console.WriteLine("5. Informe de intensidad media");
        Console.WriteLine("0. Cerrar diario\n");

        var option = InputHelper.Prompt("Selecciona una opción: ");
        switch (option)
        {
            case "1":
                await AddMemoryAsync(session);
                break;
            case "2":
                SearchMemories(session);
                break;
            case "3":
                await EditMemoryAsync(session);
                break;
            case "4":
                await DeleteMemoryAsync(session);
                break;
            case "5":
                ShowAverageIntensity(session);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opción no válida.");
                Pause();
                break;
        }
    }
}

static async Task AddMemoryAsync(DiarySession session)
{
    Console.Clear();
    Console.WriteLine("=== Añadir recuerdo ===\n");

    var date = InputHelper.PromptDate("Fecha (YYYY-MM-DD): ");
    var title = InputHelper.PromptRequired("Título: ");
    var description = InputHelper.PromptRequired("Descripción: ");
    var intensity = InputHelper.PromptInt("Intensidad (0-10): ", 0, 10);

    var entry = session.AddEntry(date, title, description, intensity);
    await session.SaveAsync();

    Console.WriteLine($"Recuerdo #{entry.Id} guardado correctamente.");
    Pause();
}

static void SearchMemories(DiarySession session)
{
    Console.Clear();
    Console.WriteLine("=== Buscar recuerdos ===\n");

    var text = InputHelper.PromptOptionalString("Texto en título o descripción (opcional): ");
    var date = InputHelper.PromptOptionalDate("Fecha exacta (YYYY-MM-DD, opcional): ");
    var intensity = InputHelper.PromptOptionalInt("Intensidad exacta (opcional): ");

    var results = session.SearchEntries(text, date, intensity).ToList();

    if (!results.Any())
    {
        Console.WriteLine("No se encontraron recuerdos con esos criterios.");
    }
    else
    {
        DisplayEntries(results);
    }

    Pause();
}

static async Task EditMemoryAsync(DiarySession session)
{
    Console.Clear();
    Console.WriteLine("=== Editar recuerdo ===\n");

    var id = InputHelper.PromptInt("Id del recuerdo a editar: ");
    var updated = session.UpdateEntry(id, entry =>
    {
        Console.WriteLine("Presiona Enter para mantener el valor actual.");
        var newDate = InputHelper.PromptOptionalDate($"Fecha ({entry.Date:yyyy-MM-dd}): ");
        if (newDate.HasValue)
        {
            entry.Date = newDate.Value;
        }

        var newTitle = InputHelper.PromptOptionalString($"Título ({entry.Title}): ");
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            entry.Title = newTitle;
        }

        var newDescription = InputHelper.PromptOptionalString($"Descripción ({entry.Description}): ");
        if (!string.IsNullOrWhiteSpace(newDescription))
        {
            entry.Description = newDescription;
        }

        var newIntensity = InputHelper.PromptOptionalInt($"Intensidad ({entry.Intensity}): ");
        if (newIntensity.HasValue)
        {
            entry.Intensity = newIntensity.Value;
        }
    });

    if (!updated)
    {
        Console.WriteLine("No se encontró un recuerdo con ese Id.");
    }
    else
    {
        await session.SaveAsync();
        Console.WriteLine("Recuerdo actualizado correctamente.");
    }

    Pause();
}

static async Task DeleteMemoryAsync(DiarySession session)
{
    Console.Clear();
    Console.WriteLine("=== Borrar recuerdo ===\n");

    var id = InputHelper.PromptInt("Id del recuerdo a borrar: ");
    var deleted = session.DeleteEntry(id);

    if (!deleted)
    {
        Console.WriteLine("No se encontró un recuerdo con ese Id.");
    }
    else
    {
        await session.SaveAsync();
        Console.WriteLine("Recuerdo borrado correctamente.");
    }

    Pause();
}

static void ShowAverageIntensity(DiarySession session)
{
    Console.Clear();
    Console.WriteLine("=== Informe de intensidad media ===\n");

    var defaultStart = DateTime.Today.AddYears(-1);
    var defaultEnd = DateTime.Today;
    Console.WriteLine($"Rango por defecto: {defaultStart:yyyy-MM-dd} a {defaultEnd:yyyy-MM-dd}\n");

    var start = InputHelper.PromptOptionalDate("Fecha de inicio (Enter para usar el valor por defecto): ") ?? defaultStart;
    var end = InputHelper.PromptOptionalDate("Fecha de fin (Enter para usar el valor por defecto): ") ?? defaultEnd;

    if (end < start)
    {
        Console.WriteLine("La fecha de fin no puede ser anterior a la fecha de inicio.");
        Pause();
        return;
    }

    var average = session.AverageIntensity(start, end);
    if (average is null)
    {
        Console.WriteLine("No hay recuerdos en ese rango.");
    }
    else
    {
        Console.WriteLine($"Intensidad media entre {start:yyyy-MM-dd} y {end:yyyy-MM-dd}: {average:0.00}");
    }

    Pause();
}

static void DisplayEntries(IEnumerable<MemoryEntry> entries)
{
    var list = entries.ToList();
    if (!list.Any())
    {
        Console.WriteLine("No hay recuerdos registrados.");
        return;
    }

    foreach (var entry in list)
    {
        Console.WriteLine($"#{entry.Id} | {entry.Date:yyyy-MM-dd} | Intensidad: {entry.Intensity}");
        Console.WriteLine($"Título: {entry.Title}");
        Console.WriteLine($"Descripción: {entry.Description}\n");
    }
}

static void Pause()
{
    Console.WriteLine("\nPresiona Enter para continuar...");
    Console.ReadLine();
}
