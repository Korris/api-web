using Mcsg.Api.Attributes;
using Mcsg.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [Authorize]
        [DisableRequestSizeLimit]
        [ServiceFilter(typeof(MediaOnlyAttribute))]
        [HttpPost("upload-media")]
        public async Task<IActionResult> UploadMedia(IFormFile file)
        {
            var result = await _fileService.UploadFileAsync(file);
            return Ok(result);
        }

        [Authorize]
        [DisableRequestSizeLimit]
        [ServiceFilter(typeof(MediaOnlyAttribute))]
        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await _fileService.UploadImageAsync(file);
            return Ok(result);
        }
    }
}