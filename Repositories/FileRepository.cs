using FileManagementSystem.Models;
using FileManagementSystem.Repositories.Interfaces;
using FileManagementSystem.Persistence;

namespace FileManagementSystem.Repositories;

public class FileRepository : IFileRepository
{
    private readonly JsonPersistenceManager _persistenceManager;
    private List<FileModel> _files = new();
    
    public FileRepository(JsonPersistenceManager persistenceManager)
    {
        _persistenceManager = persistenceManager;
    }
    
    public async Task LoadDataAsync()
    {
        var data = await _persistenceManager.LoadDataAsync();
        _files = data.Files.Where(f => !f.IsDeleted).ToList();
    }
    
    public async Task SaveDataAsync()
    {
        var data = await _persistenceManager.LoadDataAsync();
        data.Files = _files;
        await _persistenceManager.SaveDataAsync(data);
    }
    
    public async Task<FileModel?> GetByIdAsync(Guid id)
    {
        await LoadDataAsync();
        return _files.FirstOrDefault(f => f.Id == id);
    }
    
    public async Task<List<FileModel>> GetByFolderIdAsync(Guid? folderId)
    {
        await LoadDataAsync();
        return _files.Where(f => f.FolderId == folderId).ToList();
    }
    
    public async Task<List<FileModel>> GetAllAsync()
    {
        await LoadDataAsync();
        return _files.ToList();
    }
    
    public async Task<bool> ExistsInFolderAsync(string fileName, Guid? folderId, Guid? excludeFileId = null)
    {
        await LoadDataAsync();
        return _files.Any(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase) 
                              && f.FolderId == folderId 
                              && f.Id != excludeFileId);
    }
    
    public async Task<FileModel> CreateAsync(FileModel file)
    {
        await LoadDataAsync();
        _files.Add(file);
        await SaveDataAsync();
        return file;
    }
    
    public async Task<FileModel> UpdateAsync(FileModel file)
    {
        await LoadDataAsync();
        var existingIndex = _files.FindIndex(f => f.Id == file.Id);
        if (existingIndex >= 0)
        {
            file.LastModifiedAt = DateTime.UtcNow;
            _files[existingIndex] = file;
            await SaveDataAsync();
        }
        return file;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        await LoadDataAsync();
        var file = _files.FirstOrDefault(f => f.Id == id);
        if (file != null)
        {
            file.IsDeleted = true;
            await SaveDataAsync();
            await DeleteFileContentAsync(id);
            return true;
        }
        return false;
    }
    
    public async Task<byte[]?> GetFileContentAsync(Guid id)
    {
        return await _persistenceManager.LoadFileContentAsync(id);
    }
    
    public async Task SaveFileContentAsync(Guid id, byte[] content)
    {
        await _persistenceManager.SaveFileContentAsync(id, content);
    }
    
    public async Task DeleteFileContentAsync(Guid id)
    {
        await Task.Run(() => _persistenceManager.DeleteFileContent(id));
    }
}