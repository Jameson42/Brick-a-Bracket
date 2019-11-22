using BrickABracket.FileProcessing;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace BrickABracket.Controllers
{
    [Route("api/mocs")]
    public class MocImageController : Controller
    {
        private readonly MocImageService _images;
        public MocImageController(MocImageService images)
        {
            _images = images;
        }
        [HttpPost("{mocId}")]
        public async Task<IActionResult> UploadFile(int mocId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("error");
            using var readStream = file.OpenReadStream();
            if (await _images.UploadFile(mocId, readStream))
            {
                BackgroundJob.Enqueue(() => _images.ProcessImage(mocId));
                return Content("success");
            }

            return Content("error");
        }
        [HttpGet("{mocId}")]
        public async Task<IActionResult> DownloadFile(int mocId)
        {
            var image = await _images.DownloadFile(mocId);
            if (image == null)
                return NotFound();
            return File(image.File, image.Type);
        }
        [HttpGet("form")]
        public IActionResult Form()
        {
            return Content("<form action='/api/mocs/loopback' method='post' enctype='multipart/form-data'><input type='file' name='file'><input type='submit' value='submit'></form>", "text/html");
        }
        [HttpPost("loopback")]
        public async Task<IActionResult> Loopback(IFormFile file)
        {
            var memory = new MemoryStream();
            await file.CopyToAsync(memory);
            memory.Position = 0;
            return File(memory, file.ContentType);
        }
    }
}