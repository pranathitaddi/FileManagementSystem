using FileManagementSystem.DTOs;
using FileManagementSystem.Repositories.Interfaces;

namespace FileManagementSystem.Validation;

public class FolderValidator
{
    private readonly IFolderRepository _folderRepository;

    public FolderValidator(IFolderRepository folderRepository)
    {
        _folderRepository = folderRepository;
    }

    public async Task<ValidationResult> ValidateFolderCreationAsync(string name, Guid? parentFolderId)
    {
        var errors = new List<string>();

        if (!ValidationUtilities.IsValidFolderName(name))
        {
            errors.Add("Invalid folder name. Folder name cannot contain: / \\ : * ? \" < > |");
        }

        if (await _folderRepository.ExistsInParentAsync(name, parentFolderId))
        {
            errors.Add($"A folder with the name '{name}' already exists in this directory.");
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    public async Task<ValidationResult> ValidateFolderRenameAsync(Guid folderId, string newName)
    {
        var errors = new List<string>();

        if (!ValidationUtilities.IsValidFolderName(newName))
        {
            errors.Add("Invalid folder name. Folder name cannot contain: / \\ : * ? \" < > |");
        }

        var folder = await _folderRepository.GetByIdAsync(folderId);
        if (folder != null && await _folderRepository.ExistsInParentAsync(newName, folder.ParentFolderId, folderId))
        {
            errors.Add($"A folder with the name '{newName}' already exists in this directory.");
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    public async Task<ValidationResult> ValidateFolderMoveAsync(Guid folderId, Guid? newParentId)
    {
        var errors = new List<string>();

        if (newParentId.HasValue)
        {
            if (newParentId.Value == folderId)
            {
                errors.Add("Cannot move folder into itself");
            }
            else
            {
                var descendantIds = await _folderRepository.GetDescendantIdsAsync(folderId);
                if (descendantIds.Contains(newParentId.Value))
                {
                    errors.Add("Cannot move folder into its own descendant");
                }
            }
        }

        return new ValidationResult(errors.Count == 0, errors);
    }
}