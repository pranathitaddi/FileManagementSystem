using FileManagementSystem.DTOs;
using FileManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileManagementSystem.Controllers
{
    [ApiController]
    [Route("api/folders")]
    public class FoldersController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FoldersController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] FolderCreateDto createDto)
        {
            try
            {
                var folder = await _folderService.CreateFolderAsync(createDto);
                return CreatedAtAction(nameof(GetFolder), new { id = folder.Id }, folder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolder(Guid id)
        {
            var folder = await _folderService.GetFolderAsync(id);
            return folder != null ? Ok(folder) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetFolders([FromQuery] Guid? parentId)
        {
            var folders = await _folderService.GetFoldersInParentAsync(parentId);
            return Ok(folders);
        }

        [HttpGet("contents")]
        public async Task<IActionResult> GetFolderContents([FromQuery] Guid? folderId)
        {
            var contents = await _folderService.GetFolderContentsAsync(folderId);
            return contents != null ? Ok(contents) : NotFound();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFolders()
        {
            var folders = await _folderService.GetAllFoldersAsync();
            return Ok(folders);
        }

        [HttpPut("{id}/rename")]
        public async Task<IActionResult> RenameFolder(Guid id, [FromBody] FolderRenameDto renameDto)
        {
            try
            {
                var folder = await _folderService.RenameFolderAsync(id, renameDto);
                return Ok(folder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveFolder(Guid id, [FromBody] FolderMoveDto moveDto)
        {
            try
            {
                var folder = await _folderService.MoveFolderAsync(id, moveDto);
                return Ok(folder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(Guid id, [FromQuery] bool recursive = false)
        {
            try
            {
                var success = await _folderService.DeleteFolderAsync(id, recursive);
                return success ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
