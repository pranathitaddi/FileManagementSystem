namespace FileManagementSystem.Models;

public class FileModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public long Size { get; set; }
    public required string Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public Guid? FolderId { get; set; }
    public byte[]? Content { get; set; }
    public int Version { get; set; } = 1;
    public bool IsDeleted { get; set; } = false;
}