using FileManagementSystem.Models;

namespace FileManagementSystem.DTOs;

public class FileHistoryResponseDto
{
    public Guid Id { get; init; }
    public Guid FileId { get; init; }
    public DateTime Timestamp { get; init; }
    public FileActionType Action { get; init; }
    public required string Details { get; init; }
}