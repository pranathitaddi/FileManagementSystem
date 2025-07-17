namespace FileManagementSystem.Constants;

public static class FileConstants
{
    // File size limits (in bytes)
    public const long MaxFileSize = 100 * 1024 * 1024; // 100MB
    
    // Blocked file extensions for security
    public static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".bat", ".cmd", ".com", ".pif", ".scr", ".vbs", ".js", ".jar", ".msi"
    };
    
    // Allowed MIME types (extensible)
    public static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/bmp", "image/svg+xml",
        "text/plain", "text/html", "text/css", "text/javascript",
        "application/pdf", "application/json", "application/xml",
        "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "video/mp4", "video/avi", "video/quicktime",
        "audio/mpeg", "audio/wav", "audio/ogg"
    };
    
    // Naming constraints
    public const int MaxFileNameLength = 255;
    public const int MaxFolderNameLength = 255;
    
    // Characters not allowed in file/folder names
    public static readonly char[] InvalidNameChars = { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
    
    // Persistence settings
    public const string DataFileName = "filemanagement_data.json";
    public const string StorageDirectory = "FileStorage";
}