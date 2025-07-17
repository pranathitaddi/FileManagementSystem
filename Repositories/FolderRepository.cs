using FileManagementSystem.Models;
using FileManagementSystem.Repositories.Interfaces;
using FileManagementSystem.Persistence;

namespace FileManagementSystem.Repositories;

public class FolderRepository : IFolderRepository
{
    private readonly JsonPersistenceManager _persistenceManager;
    private List<FolderModel> _folders = new();
    
    public FolderRepository(JsonPersistenceManager persistenceManager)
    {
        _persistenceManager = persistenceManager;
    }
    
    public async Task LoadDataAsync()
    {
        var data = await _persistenceManager.LoadDataAsync();
        _folders = data.Folders.Where(f => !f.IsDeleted).ToList();
    }
    
    public async Task SaveDataAsync()
    {
        var data = await _persistenceManager.LoadDataAsync();
        data.Folders = _folders;
        await _persistenceManager.SaveDataAsync(data);
    }
    
    public async Task<FolderModel?> GetByIdAsync(Guid id)
    {
        await LoadDataAsync();
        return _folders.FirstOrDefault(f => f.Id == id);
    }
    
    public async Task<List<FolderModel>> GetByParentIdAsync(Guid? parentId)
    {
        await LoadDataAsync();
        return _folders.Where(f => f.ParentFolderId == parentId).ToList();
    }
    
    public async Task<List<FolderModel>> GetSubfoldersAsync(Guid? parentId)
    {
        await LoadDataAsync();
        return _folders.Where(f => f.ParentFolderId == parentId).ToList();
    }

    public async Task<List<FolderModel>> GetAllAsync()
    {
        await LoadDataAsync();
        return _folders.ToList();
    }
    
    public async Task<bool> ExistsInParentAsync(string folderName, Guid? parentId, Guid? excludeFolderId = null)
    {
        await LoadDataAsync();
        return _folders.Any(f => f.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase) 
                                && f.ParentFolderId == parentId 
                                && f.Id != excludeFolderId);
    }
    
    public async Task<FolderModel> CreateAsync(FolderModel folder)
    {
        await LoadDataAsync();
        _folders.Add(folder);
        await SaveDataAsync();
        return folder;
    }
    
    public async Task<FolderModel> UpdateAsync(FolderModel folder)
    {
        await LoadDataAsync();
        var existingIndex = _folders.FindIndex(f => f.Id == folder.Id);
        if (existingIndex >= 0)
        {
            folder.LastModifiedAt = DateTime.UtcNow;
            _folders[existingIndex] = folder;
            await SaveDataAsync();
        }
        return folder;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        await LoadDataAsync();
        var folder = _folders.FirstOrDefault(f => f.Id == id);
        if (folder != null)
        {
            folder.IsDeleted = true;
            await SaveDataAsync();
            return true;
        }
        return false;
    }
    
    public async Task<List<Guid>> GetDescendantIdsAsync(Guid folderId)
    {
        await LoadDataAsync();
        var descendants = new List<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(folderId);
        
        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = _folders.Where(f => f.ParentFolderId == currentId).ToList();
            
            foreach (var child in children)
            {
                descendants.Add(child.Id);
                queue.Enqueue(child.Id);
            }
        }
        
        return descendants;
    }
    
    public async Task<bool> IsEmptyAsync(Guid folderId)
    {
        await LoadDataAsync();
        // Check if folder has any child folders
        var hasChildFolders = _folders.Any(f => f.ParentFolderId == folderId);
        return !hasChildFolders;
    }

    
}