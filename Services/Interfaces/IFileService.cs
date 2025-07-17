using FileManagementSystem.DTOs;
using FileManagementSystem.Models;

namespace FileManagementSystem.Services.Interfaces;

public interface IFileService
{
    Task<FileResponseDto> UploadFileAsync(FileUploadDto uploadDto, byte[] content);
    Task<FileDownloadDto?> GetFileAsync(Guid id);
    Task<FileResponseDto?> GetFileMetadataAsync(Guid id);
    Task<List<FileResponseDto>> GetFilesInFolderAsync(Guid? folderId);
    Task<FileResponseDto> RenameFileAsync(Guid id, FileRenameDto renameDto);
    Task<FileResponseDto> MoveFileAsync(Guid id, FileMoveDto moveDto);
    Task<bool> DeleteFileAsync(Guid id);
}