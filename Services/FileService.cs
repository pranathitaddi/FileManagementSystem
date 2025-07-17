using FileManagementSystem.DTOs;
using FileManagementSystem.Models;
using FileManagementSystem.Services.Interfaces;
using FileManagementSystem.Repositories.Interfaces;
using FileManagementSystem.Validation;
using System.Text.Json;

namespace FileManagementSystem.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IFolderRepository _folderRepository;
    private readonly IFileHistoryService _historyService;
    
    public FileService(
        IFileRepository fileRepository,
        IFolderRepository folderRepository,
        IFileHistoryService historyService)
    {
        _fileRepository = fileRepository;
        _folderRepository = folderRepository;
        _historyService = historyService;
    }
    
    public async Task<FileResponseDto> UploadFileAsync(FileUploadDto uploadDto, byte[] content)
    {
        // Validate input
        var validation = FileValidator.ValidateFileUpload(uploadDto, content.Length);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }
        
        // Check if parent folder exists
        if (uploadDto.FolderId.HasValue)
        {
            var parentFolder = await _folderRepository.GetByIdAsync(uploadDto.FolderId.Value);
            if (parentFolder == null)
            {
                throw new ArgumentException("Parent folder not found");
            }
        }
        
        // Check for duplicate names in the same folder
        if (await _fileRepository.ExistsInFolderAsync(uploadDto.Name, uploadDto.FolderId))
        {
            throw new InvalidOperationException($"File with name '{uploadDto.Name}' already exists in this folder");
        }
        
        // Create file model
        var file = new FileModel
        {
            Name = uploadDto.Name,
            Size = content.Length,
            Type = uploadDto.Type,
            FolderId = uploadDto.FolderId
        };
        
        // Save file content
        await _fileRepository.SaveFileContentAsync(file.Id, content);
        
        // Save file metadata
        var createdFile = await _fileRepository.CreateAsync(file);
        
        // Log action
        await _historyService.LogFileActionAsync(
            createdFile.Id,
            FileActionType.Uploaded.ToString(),
            JsonSerializer.Serialize(new { FileName = uploadDto.Name, Size = content.Length }),
            null);
        
        return MapToResponseDto(createdFile);
    }
    
    public async Task<FileDownloadDto?> GetFileAsync(Guid id)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        if (file == null)
            return null;
        
        var content = await _fileRepository.GetFileContentAsync(id);
        if (content == null)
            return null;
        
        return new FileDownloadDto
        {
            Name = file.Name,
            Type = file.Type,
            Content = content
        };
    }
    
    public async Task<FileResponseDto?> GetFileMetadataAsync(Guid id)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        return file != null ? MapToResponseDto(file) : null;
    }
    
    public async Task<List<FileResponseDto>> GetFilesInFolderAsync(Guid? folderId)
    {
        var files = await _fileRepository.GetByFolderIdAsync(folderId);
        return files.Select(MapToResponseDto).ToList();
    }
    
    public async Task<FileResponseDto> RenameFileAsync(Guid id, FileRenameDto renameDto)
    {
        var validation = FileValidator.ValidateFileRename(renameDto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }
        
        var file = await _fileRepository.GetByIdAsync(id);
        if (file == null)
        {
            throw new ArgumentException("File not found");
        }
        
        // Check for duplicate names in the same folder
        if (await _fileRepository.ExistsInFolderAsync(renameDto.NewName, file.FolderId, id))
        {
            throw new InvalidOperationException($"File with name '{renameDto.NewName}' already exists in this folder");
        }
        
        var oldName = file.Name;
        file.Name = renameDto.NewName;
        
        var updatedFile = await _fileRepository.UpdateAsync(file);
        
        // Log action
        await _historyService.LogFileActionAsync(
            id,
            FileActionType.Renamed.ToString(),
            JsonSerializer.Serialize(new { OldName = oldName, NewName = renameDto.NewName }),
            null);
        
        return MapToResponseDto(updatedFile);
    }
    
    public async Task<FileResponseDto> MoveFileAsync(Guid id, FileMoveDto moveDto)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        if (file == null)
        {
            throw new ArgumentException("File not found");
        }
        
        // Check if destination folder exists
        if (moveDto.NewFolderId.HasValue)
        {
            var destinationFolder = await _folderRepository.GetByIdAsync(moveDto.NewFolderId.Value);
            if (destinationFolder == null)
            {
                throw new ArgumentException("Destination folder not found");
            }
        }
        
        // Check for duplicate names in the destination folder
        if (await _fileRepository.ExistsInFolderAsync(file.Name, moveDto.NewFolderId, id))
        {
            throw new InvalidOperationException($"File with name '{file.Name}' already exists in the destination folder");
        }
        
        var oldFolderId = file.FolderId;
        file.FolderId = moveDto.NewFolderId;
        
        var updatedFile = await _fileRepository.UpdateAsync(file);
        
        // Log action
        await _historyService.LogFileActionAsync(
            id,
            FileActionType.Moved.ToString(),
            JsonSerializer.Serialize(new { OldFolderId = oldFolderId, NewFolderId = moveDto.NewFolderId }),
            null);
        
        return MapToResponseDto(updatedFile);
    }
    
    public async Task<bool> DeleteFileAsync(Guid id)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        if (file == null)
            return false;
        
        var success = await _fileRepository.DeleteAsync(id);
        
        if (success)
        {
            // Log action
            await _historyService.LogFileActionAsync(
                id,
                FileActionType.Deleted.ToString(),
                JsonSerializer.Serialize(new { FileName = file.Name }),
                null);
        }
        
        return success;
    }
    
    private static FileResponseDto MapToResponseDto(FileModel file)
    {
        return new FileResponseDto
        {
            Id = file.Id,
            Name = file.Name,
            Size = file.Size,
            Type = file.Type,
            CreatedAt = file.CreatedAt,
            LastModifiedAt = file.LastModifiedAt,
            FolderId = file.FolderId,
            Version = file.Version
        };
    }
}