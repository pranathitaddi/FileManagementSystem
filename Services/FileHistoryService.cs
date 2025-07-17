using FileManagementSystem.DTOs;
using FileManagementSystem.Models;
using FileManagementSystem.Repositories.Interfaces;
using FileManagementSystem.Services.Interfaces;

namespace FileManagementSystem.Services
{
    public class FileHistoryService : IFileHistoryService
    {
        private readonly IFileHistoryRepository _historyRepository;

        public FileHistoryService(IFileHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task LogFileActionAsync(Guid fileId, string action, string details, string? performedBy = null)
        {
            if (!Enum.TryParse<FileActionType>(action, ignoreCase: true, out var actionEnum))
            {
                throw new ArgumentException($"Invalid file action type: {action}", nameof(action));
            }

            var historyEntry = new FileHistoryModel
            {
                FileId = fileId,
                Action = actionEnum,
                Details = details
            };

            await _historyRepository.CreateAsync(historyEntry);
        }

        public async Task<List<FileHistoryResponseDto>> GetFileHistoryAsync(Guid fileId)
        {
            var historyModels = await _historyRepository.GetByFileIdAsync(fileId);
            
            var historyDtos = historyModels.Select(h => new FileHistoryResponseDto
            {
                Id = h.Id,
                FileId = h.FileId,
                Timestamp = h.Timestamp,
                Action = h.Action,
                Details = h.Details
            }).ToList();

            return historyDtos;
        }
    }
}