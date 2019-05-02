using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BrickABracket.Core.Services;

namespace BrickABracket.Controllers
{
    [Route("api/mocs")]
    public class MocImageController : Controller
    {
        private MocImageService _images;
        private const string pathPrefix = "$/mocs/";
        public MocImageController(MocImageService images)
        {
            _images = images;
        }
        [HttpPost("{mocId}")]
        public async Task<IActionResult> UploadFile(int mocId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("error");
            var path = pathPrefix + mocId;
            var filename = "" + mocId;
            using (var stream = _images.WriteStream(path, filename))
            {
                await file.CopyToAsync(stream);
            }
            return Content("success");
        }
        [HttpGet("{mocId}")]
        public async Task<IActionResult> DownloadFile(int mocId)
        {
            if (mocId<1)
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
            return File(memory, fileInfo.MimeType,fileInfo.Filename);
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