namespace FileManagementSystem.Models;

public class FolderModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public Guid? ParentFolderId { get; set; } 
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    
    public List<FileModel> Files { get; set; } = new();
    public List<FolderModel> SubFolders { get; set; } = new();
}