using FileManagementSystem.Models;

namespace FileManagementSystem.Repositories.Interfaces;

public interface IFileRepository
{
    Task<FileModel?> GetByIdAsync(Guid id);
    Task<List<FileModel>> GetByFolderIdAsync(Guid? folderId);
    Task<List<FileModel>> GetAllAsync();
    Task<bool> ExistsInFolderAsync(string fileName, Guid? folderId, Guid? excludeFileId = null);
    Task<FileModel> CreateAsync(FileModel file);
    Task<FileModel> UpdateAsync(FileModel file);
    Task<bool> DeleteAsync(Guid id);
    Task<byte[]?> GetFileContentAsync(Guid id);
    Task SaveFileContentAsync(Guid id, byte[] content);
    Task DeleteFileContentAsync(Guid id);
}