using System.Security.Cryptography;
using System.Text.Json;
using MemoryLedgerApp.Models;
using MemoryLedgerApp.Utilities;

namespace MemoryLedgerApp.Services;

public class DiaryStorage
{
    private readonly string _diariesDirectory;

    public DiaryStorage(string rootPath)
    {
        _diariesDirectory = Path.Combine(rootPath, "diaries");
        Directory.CreateDirectory(_diariesDirectory);
    }

    public IEnumerable<string> ListDiaries() =>
        Directory
            .EnumerateFiles(_diariesDirectory, "*.mlg", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .OrderBy(name => name);

    public bool DiaryExists(string name) => File.Exists(GetDiaryPath(name));

    public async Task CreateDiaryAsync(string name, string password, CancellationToken cancellationToken = default)
    {
        if (DiaryExists(name))
        {
            throw new InvalidOperationException($"A diary named '{name}' already exists.");
        }

        var diary = new Diary { Name = name };
        await SaveDiaryAsync(diary, password, cancellationToken);
    }

    public async Task<Diary> LoadDiaryAsync(string name, string password, CancellationToken cancellationToken = default)
    {
        var path = GetDiaryPath(name);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The requested diary does not exist.", path);
        }

        var encryptedBytes = await File.ReadAllBytesAsync(path, cancellationToken);
        try
        {
            var json = EncryptionService.Decrypt(encryptedBytes, password);
            var diary = JsonSerializer.Deserialize<Diary>(json, SerializerOptions);
            return diary ?? throw new InvalidDataException("Unable to read the diary content.");
        }
        catch (CryptographicException)
        {
            throw new UnauthorizedAccessException("The provided password is incorrect.");
        }
    }

    public async Task SaveDiaryAsync(Diary diary, string password, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(diary, SerializerOptions);
        var encrypted = EncryptionService.Encrypt(json, password);
        var path = GetDiaryPath(diary.Name);
        await File.WriteAllBytesAsync(path, encrypted, cancellationToken);
    }

    public void DeleteDiary(string name)
    {
        var path = GetDiaryPath(name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private string GetDiaryPath(string name) => Path.Combine(_diariesDirectory, $"{SanitizeFileName(name)}.mlg");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static string SanitizeFileName(string name)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return name.Trim();
    }
}
