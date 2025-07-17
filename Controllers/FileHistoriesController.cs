using FileManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileManagementSystem.Controllers
{
    [ApiController]
    [Route("api/history")]
    public class FileHistoriesController : ControllerBase
    {
        private readonly IFileHistoryService _fileHistoryService;

        public FileHistoriesController(IFileHistoryService fileHistoryService)
        {
            _fileHistoryService = fileHistoryService;
        }

        [HttpGet("file/{fileId}")]
        public async Task<IActionResult> GetFileHistory(Guid fileId)
        {
            var history = await _fileHistoryService.GetFileHistoryAsync(fileId);
            return Ok(history);
        }
    }
}
