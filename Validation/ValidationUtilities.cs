using FileManagementSystem.Constants;

namespace FileManagementSystem.Validation;

public static class ValidationUtilities
{
    public static bool IsValidFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;
            
        if (fileName.Length > FileConstants.MaxFileNameLength)
            return false;
            
        return !fileName.Any(c => FileConstants.InvalidNameChars.Contains(c));
    }
    
    public static bool IsValidFolderName(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return false;
            
        if (folderName.Length > FileConstants.MaxFolderNameLength)
            return false;
            
        return !folderName.Any(c => FileConstants.InvalidNameChars.Contains(c));
    }
    
    public static bool IsAllowedFileType(string fileName, string mimeType)
    {
        var extension = Path.GetExtension(fileName);
        
        // Check if extension is blocked
        if (!string.IsNullOrEmpty(extension) && FileConstants.BlockedExtensions.Contains(extension))
            return false;
            
        // Check if MIME type is allowed
        return FileConstants.AllowedMimeTypes.Contains(mimeType);
    }
    
    public static bool IsValidFileSize(long size)
    {
        return size > 0 && size <= FileConstants.MaxFileSize;
    }
    
    public static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant();
    }
    
    public static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "unnamed_file";
            
        // Replace invalid characters with underscores
        var sanitized = fileName;
        foreach (var invalidChar in FileConstants.InvalidNameChars)
        {
            sanitized = sanitized.Replace(invalidChar, '_');
        }
        
        return sanitized.Trim();
    }
}