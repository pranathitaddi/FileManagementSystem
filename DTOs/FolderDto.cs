namespace FileManagementSystem.DTOs;

public class FolderCreateDto
{
    public required string Name { get; set; }
    public Guid? ParentFolderId { get; init; }
}

public class FolderRenameDto
{
    public required string NewName { get; set; }
}

public class FolderMoveDto
{
    public Guid? NewParentFolderId { get; set; }
}

public class FolderResponseDto
{
    public Guid Id { get; init; }
    public required string Name { get; set; }
    public Guid? ParentFolderId { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastModifiedAt { get; init; }
    public int FileCount { get; set; }
    public int SubFolderCount { get; set; }
}

public class FolderContentsDto
{
    public FolderResponseDto Folder { get; set; } = null!;
    public List<FileResponseDto> Files { get; set; } = new();
    public List<FolderResponseDto> SubFolders { get; set; } = new();
}