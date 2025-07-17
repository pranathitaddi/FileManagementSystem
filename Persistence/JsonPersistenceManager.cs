using System.Text.Json;
using FileManagementSystem.Models;
using FileManagementSystem.Constants;

namespace FileManagementSystem.Persistence;

public class JsonPersistenceManager
{
    private readonly string _dataFilePath;
    private readonly string _storageDirectory;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public JsonPersistenceManager()
    {
        _dataFilePath = Path.Combine(Directory.GetCurrentDirectory(), FileConstants.DataFileName);
        _storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileConstants.StorageDirectory);
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        // Ensure storage directory exists
        Directory.CreateDirectory(_storageDirectory);
    }
    
    public async Task<DataContainer> LoadDataAsync()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new DataContainer();
        }
        
        try
        {
            var json = await File.ReadAllTextAsync(_dataFilePath);
            var data = JsonSerializer.Deserialize<DataContainer>(json, _jsonOptions);
            return data ?? new DataContainer();
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging
            Console.WriteLine($"Error loading data: {ex.Message}");
            return new DataContainer();
        }
    }
    
    public async Task SaveDataAsync(DataContainer data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            await File.WriteAllTextAsync(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging
            Console.WriteLine($"Error saving data: {ex.Message}");
            throw;
        }
    }
    
    public async Task<string> SaveFileContentAsync(Guid fileId, byte[] content)
    {
        var filePath = Path.Combine(_storageDirectory, $"{fileId}.bin");
        await File.WriteAllBytesAsync(filePath, content);
        return filePath;
    }
    
    public async Task<byte[]?> LoadFileContentAsync(Guid fileId)
    {
        var filePath = Path.Combine(_storageDirectory, $"{fileId}.bin");
        if (!File.Exists(filePath))
            return null;
            
        return await File.ReadAllBytesAsync(filePath);
    }
    
    public void DeleteFileContent(Guid fileId)
    {
        var filePath = Path.Combine(_storageDirectory, $"{fileId}.bin");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}

// Data container for JSON serialization
public class DataContainer
{
    public List<FileModel> Files { get; set; } = new();
    public List<FolderModel> Folders { get; set; } = new();
    public List<FileHistoryModel> FileHistory { get; set; } = new();
}