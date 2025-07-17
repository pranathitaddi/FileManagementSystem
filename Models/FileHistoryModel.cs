namespace FileManagementSystem.Models;

public class FileHistoryModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid FileId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public FileActionType Action { get; set; }
    public required string Details { get; set; }
}

// Enum for common actions to ensure consistency
public enum FileActionType
{
    Uploaded,
    Renamed,
    Moved,
    Deleted,
    Restored
}
