using FileManagementSystem.DTOs;
using FileManagementSystem.Models;
using FileManagementSystem.Services.Interfaces;
using FileManagementSystem.Repositories.Interfaces;
using FileManagementSystem.Validation;

namespace FileManagementSystem.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IFileRepository _fileRepository;
        private readonly FolderValidator _validator;

        public FolderService(
            IFolderRepository folderRepository,
            IFileRepository fileRepository,
            FolderValidator validator)
        {
            _folderRepository = folderRepository;
            _fileRepository = fileRepository;

            _validator = validator;
        }

        public async Task<FolderResponseDto> CreateFolderAsync(FolderCreateDto createDto)
        {
            var validationResult = await _validator.ValidateFolderCreationAsync(createDto.Name, createDto.ParentFolderId);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(string.Join(", ", validationResult.Errors));
            }

            var folder = new FolderModel
            {
                Name = createDto.Name,
                ParentFolderId = createDto.ParentFolderId,
            };

            var createdFolder = await _folderRepository.CreateAsync(folder);
            await _folderRepository.LogActionAsync(null, createdFolder.Id, "Created", $"Folder '{createdFolder.Name}' created");

            return await MapToResponseDto(createdFolder);
        }

        public async Task<FolderResponseDto?> GetFolderAsync(Guid id)
        {
            var folder = await _folderRepository.GetByIdAsync(id);
            return folder != null ? await MapToResponseDto(folder) : null;
        }

        public async Task<List<FolderResponseDto>> GetFoldersInParentAsync(Guid? parentId)
        {
            var folders = await _folderRepository.GetSubfoldersAsync(parentId);
            var dtos = new List<FolderResponseDto>();
            foreach (var folder in folders)
            {
                dtos.Add(await MapToResponseDto(folder));
            }
            return dtos;
        }

        public async Task<FolderContentsDto?> GetFolderContentsAsync(Guid? folderId)
        {
            FolderModel? folder = null;
            if (folderId.HasValue)
            {
                folder = await _folderRepository.GetByIdAsync(folderId.Value);
                if (folder == null)
                {
                    return null;
                }
            }

            var files = await _fileRepository.GetByFolderIdAsync(folderId);
            var subFolders = await _folderRepository.GetSubfoldersAsync(folderId);

            var fileDtos = files.Select(f => new FileResponseDto
            {
                Id = f.Id,
                Name = f.Name,
                Size = f.Size,
                Type = f.Type,
                CreatedAt = f.CreatedAt,
                LastModifiedAt = f.LastModifiedAt,
                FolderId = f.FolderId,
                Version = f.Version
            }).ToList();

            var subFolderDtos = new List<FolderResponseDto>();
            foreach (var subFolder in subFolders)
            {
                subFolderDtos.Add(await MapToResponseDto(subFolder));
            }

            var folderDto = folder != null ? await MapToResponseDto(folder) : new FolderResponseDto { Name = "Root", Id = Guid.Empty, CreatedAt = DateTime.UtcNow, LastModifiedAt = DateTime.UtcNow, ParentFolderId = null, FileCount = 0, SubFolderCount = 0 };

            return new FolderContentsDto
            {
                Folder = folderDto,
                Files = fileDtos,
                SubFolders = subFolderDtos
            };
        }

        public async Task<List<FolderResponseDto>> GetAllFoldersAsync()
        {
            var folders = await _folderRepository.GetAllAsync();
            var dtos = new List<FolderResponseDto>();
            foreach (var folder in folders)
            {
                dtos.Add(await MapToResponseDto(folder));
            }
            return dtos;
        }

        public async Task<FolderResponseDto> RenameFolderAsync(Guid id, FolderRenameDto renameDto)
        {
            var validationResult = await _validator.ValidateFolderRenameAsync(id, renameDto.NewName);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(string.Join(", ", validationResult.Errors));
            }

            var folder = (await _folderRepository.GetByIdAsync(id))!;
            var oldName = folder.Name;
            folder.Name = renameDto.NewName;

            var updatedFolder = await _folderRepository.UpdateAsync(folder);
            await _historyService.LogActionAsync(null, updatedFolder.Id, "Renamed", $"Folder renamed from '{oldName}' to '{updatedFolder.Name}'");

            return await MapToResponseDto(updatedFolder);
        }

        public async Task<FolderResponseDto> MoveFolderAsync(Guid id, FolderMoveDto moveDto)
        {
            var validationResult = await _validator.ValidateFolderMoveAsync(id, moveDto.NewParentFolderId);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(string.Join(", ", validationResult.Errors));
            }

            var folder = (await _folderRepository.GetByIdAsync(id))!;
            var oldParentId = folder.ParentFolderId;
            folder.ParentFolderId = moveDto.NewParentFolderId;

            var updatedFolder = await _folderRepository.UpdateAsync(folder);

            var oldParentName = oldParentId.HasValue ? (await _folderRepository.GetByIdAsync(oldParentId.Value))?.Name ?? "Unknown" : "Root";
            var newParentName = moveDto.NewParentFolderId.HasValue ? (await _folderRepository.GetByIdAsync(moveDto.NewParentFolderId.Value))?.Name ?? "Unknown" : "Root";

            await _historyService.LogActionAsync(null, updatedFolder.Id, "Moved", $"Folder moved from '{oldParentName}' to '{newParentName}'");

            return await MapToResponseDto(updatedFolder);
        }

        public async Task<bool> DeleteFolderAsync(Guid id, bool recursive = false)
        {
            var folder = await _folderRepository.GetByIdAsync(id);
            if (folder == null)
            {
                return false;
            }

            var subfolders = await _folderRepository.GetSubfoldersAsync(id);
            var files = await _fileRepository.GetByFolderIdAsync(id);

            if ((subfolders.Any() || files.Any()) && !recursive)
            {
                throw new InvalidOperationException("Cannot delete folder with contents. Use recursive delete or move contents first.");
            }

            if (recursive)
            {
                foreach (var file in files)
                {
                    await _fileRepository.DeleteAsync(file.Id);
                    await _historyService.LogActionAsync(file.Id, null, "Deleted", "File deleted due to parent folder deletion");
                }

                foreach (var subfolder in subfolders)
                {
                    await DeleteFolderAsync(subfolder.Id, true);
                }
            }

            await _folderRepository.DeleteAsync(id);
            await _historyService.LogActionAsync(null, id, "Deleted", $"Folder '{folder.Name}' deleted");

            return true;
        }

        private async Task<FolderResponseDto> MapToResponseDto(FolderModel folder)
        {
            var fileCount = (await _fileRepository.GetByFolderIdAsync(folder.Id)).Count;
            var subFolderCount = (await _folderRepository.GetSubfoldersAsync(folder.Id)).Count();
            return new FolderResponseDto
            {
                Id = folder.Id,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId,
                CreatedAt = folder.CreatedAt,
                LastModifiedAt = folder.LastModifiedAt,
                FileCount = fileCount,
                SubFolderCount = subFolderCount
            };
        }
    }
}