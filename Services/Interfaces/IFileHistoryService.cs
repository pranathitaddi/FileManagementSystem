using FileManagementSystem.DTOs;

namespace FileManagementSystem.Services.Interfaces;

public interface IFileHistoryService
{
    Task<List<FileHistoryResponseDto>> GetFileHistoryAsync(Guid fileId);
    Task LogFileActionAsync(Guid fileId, string action, string details, string? performedBy = null);
}