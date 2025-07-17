using FileManagementSystem.DTOs;

namespace FileManagementSystem.Services.Interfaces;

public interface IFolderService
{
    Task<FolderResponseDto> CreateFolderAsync(FolderCreateDto createDto);
    Task<FolderResponseDto?> GetFolderAsync(Guid id);
    Task<List<FolderResponseDto>> GetFoldersInParentAsync(Guid? parentId);
    Task<FolderContentsDto?> GetFolderContentsAsync(Guid? folderId);
    Task<List<FolderResponseDto>> GetAllFoldersAsync();
    Task<FolderResponseDto> RenameFolderAsync(Guid id, FolderRenameDto renameDto);
    Task<FolderResponseDto> MoveFolderAsync(Guid id, FolderMoveDto moveDto);
    Task<bool> DeleteFolderAsync(Guid id, bool recursive = false);
}