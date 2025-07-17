using FileManagementSystem.DTOs;
using FileManagementSystem.Constants;

namespace FileManagementSystem.Validation;

public class FileValidator
{
    public static ValidationResult ValidateFileUpload(FileUploadDto dto, long contentSize)
    {
        var errors = new List<string>();
        
        // Validate file name
        if (!ValidationUtilities.IsValidFileName(dto.Name))
        {
            errors.Add("Invalid file name. File name cannot contain: / \\ : * ? \" < > |");
        }
        
        // Validate file size
        if (!ValidationUtilities.IsValidFileSize(contentSize))
        {
            errors.Add($"File size must be between 1 byte and {FileConstants.MaxFileSize / (1024 * 1024)}MB");
        }
        
        // Validate file type
        if (!ValidationUtilities.IsAllowedFileType(dto.Name, dto.Type))
        {
            errors.Add("File type not allowed or blocked for security reasons");
        }
        
        return new ValidationResult(errors.Count == 0, errors);
    }
    
    public static ValidationResult ValidateFileRename(FileRenameDto dto)
    {
        var errors = new List<string>();
        
        if (!ValidationUtilities.IsValidFileName(dto.NewName))
        {
            errors.Add("Invalid file name. File name cannot contain: / \\ : * ? \" < > |");
        }
        
        return new ValidationResult(errors.Count == 0, errors);
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public List<string> Errors { get; }
    
    public ValidationResult(bool isValid, List<string> errors)
    {
        IsValid = isValid;
        Errors = errors ?? new List<string>();
    }
}