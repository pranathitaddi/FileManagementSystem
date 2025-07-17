namespace FileManagementSystem.DTOs;

public class FileUploadDto
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public Guid? FolderId { get; set; }
    public long? Size { get; set; }
}

public class FileRenameDto
{
    public required string NewName { get; set; }
}

public class FileMoveDto
{
    public Guid? NewFolderId { get; set; } 
}

public class FileResponseDto
{
    public Guid Id { get; init; }
    public required string Name { get; set; }
    public long Size { get; set; }
    public required string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public Guid? FolderId { get; set; }
    public int Version { get; set; }
}

public class FileDownloadDto
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
}