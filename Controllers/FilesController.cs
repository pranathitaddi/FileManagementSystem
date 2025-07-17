using FileManagementSystem.DTOs;
using FileManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileManagementSystem.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(
            [FromForm] FileUploadDto metadata,
            [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File content is missing");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var content = memoryStream.ToArray();

            var response = await _fileService.UploadFileAsync(metadata, content);
            return CreatedAtAction(nameof(GetFileMetadata), new { id = response.Id }, response);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var file = await _fileService.GetFileAsync(id);
            if (file == null)
                return NotFound();

            return File(file.Content, file.Type, file.Name);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileMetadata(Guid id)
        {
            var file = await _fileService.GetFileMetadataAsync(id);
            return file != null ? Ok(file) : NotFound();
        }

        [HttpGet("folder")]
        public async Task<IActionResult> GetFilesInFolder([FromQuery] Guid? folderId)
        {
            var files = await _fileService.GetFilesInFolderAsync(folderId);
            return Ok(files);
        }

        [HttpPut("{id}/rename")]
        public async Task<IActionResult> RenameFile(Guid id, [FromBody] FileRenameDto renameDto)
        {
            try
            {
                var response = await _fileService.RenameFileAsync(id, renameDto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveFile(Guid id, [FromBody] FileMoveDto moveDto)
        {
            try
            {
                var response = await _fileService.MoveFileAsync(id, moveDto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var success = await _fileService.DeleteFileAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
