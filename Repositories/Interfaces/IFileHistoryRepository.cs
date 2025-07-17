using FileManagementSystem.Models;

namespace FileManagementSystem.Repositories.Interfaces;

public interface IFileHistoryRepository
{
    Task<List<FileHistoryModel>> GetByFileIdAsync(Guid fileId);
    Task<FileHistoryModel> CreateAsync(FileHistoryModel history);
    Task<List<FileHistoryModel>> GetAllAsync();
}