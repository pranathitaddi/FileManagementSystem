using FileManagementSystem.Models;

namespace FileManagementSystem.Repositories.Interfaces;

public interface IFolderRepository
{
    Task<FolderModel?> GetByIdAsync(Guid id);
    Task<List<FolderModel>> GetByParentIdAsync(Guid? parentId);
    Task<List<FolderModel>> GetSubfoldersAsync(Guid? parentId);
    Task<List<FolderModel>> GetAllAsync();
    Task<bool> ExistsInParentAsync(string folderName, Guid? parentId, Guid? excludeFolderId = null);
    Task<FolderModel> CreateAsync(FolderModel folder);
    Task<FolderModel> UpdateAsync(FolderModel folder);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Guid>> GetDescendantIdsAsync(Guid folderId);
    Task<bool> IsEmptyAsync(Guid folderId);
}