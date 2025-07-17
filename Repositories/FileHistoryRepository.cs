using FileManagementSystem.Models;
using FileManagementSystem.Repositories.Interfaces;
using FileManagementSystem.Persistence;

namespace FileManagementSystem.Repositories;

public class FileHistoryRepository : IFileHistoryRepository
{
    private readonly JsonPersistenceManager _persistenceManager;
    private List<FileHistoryModel> _history = new();
    
    public FileHistoryRepository(JsonPersistenceManager persistenceManager)
    {
        _persistenceManager = persistenceManager;
    }
    
    public async Task LoadDataAsync()
    {
        var data = await _persistenceManager.LoadDataAsync();
        _history = data.FileHistory;
    }
    
    public async Task SaveDataAsync()
    {
        var data = await _persistenceManager.LoadDataAsync();
        data.FileHistory = _history;
        await _persistenceManager.SaveDataAsync(data);
    }
    
    public async Task<List<FileHistoryModel>> GetByFileIdAsync(Guid fileId)
    {
        await LoadDataAsync();
        return _history.Where(h => h.FileId == fileId)
                      .OrderByDescending(h => h.Timestamp)
                      .ToList();
    }
    
    public async Task<FileHistoryModel> CreateAsync(FileHistoryModel history)
    {
        await LoadDataAsync();
        _history.Add(history);
        await SaveDataAsync();
        return history;
    }
    
    public async Task<List<FileHistoryModel>> GetAllAsync()
    {
        await LoadDataAsync();
        return _history.OrderByDescending(h => h.Timestamp).ToList();
    }
}