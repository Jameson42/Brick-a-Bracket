using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using BrickABracket.Core.Services;

namespace BrickABracket.Controllers
{
    [Route("api/mocs")]
    public class MocImageController : Controller
    {
        private readonly MocImageService _images;
        private const string pathPrefix = "$/mocs/";
        public MocImageController(MocImageService images)
        {
            _images = images;
        }
        [HttpPost("{mocId}")]
        public IActionResult UploadFile(int mocId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("error");
            var path = pathPrefix + mocId;
            var filename = "" + mocId;
            using (var readStream = file.OpenReadStream())
            {
                using var image = Image.Load(readStream);
                // Probably worth experimenting with later, crop based on entropy threshold
                // image.Mutate(ctx => ctx.EntropyCrop());

                image.Mutate(c => c.AutoOrient());
                image.Mutate(c => c.Resize(640, 0));
                using var writeStream = _images.WriteStream(path, filename);
                image.SaveAsJpeg(writeStream);
            }
            return Content("success");
        }
        [HttpGet("{mocId}")]
        public async Task<IActionResult> DownloadFile(int mocId)
        {
            if (mocId < 1)
                return Content("error - invalid id");
            var path = pathPrefix + mocId;
            var memory = new MemoryStream();
            var fileInfo = _images.ReadFileInfo(path);
            if (fileInfo == null)
                return Content("error - no image");
            using (var stream = _images.ReadStream(path))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "image/jpeg", fileInfo.Filename + ".jpg");
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