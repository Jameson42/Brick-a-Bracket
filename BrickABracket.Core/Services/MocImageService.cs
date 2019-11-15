using System;
using System.IO;
using System.Threading.Tasks;
using LiteDB;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocImageService : IDisposable
    {
        private LiteRepository Db { get; }
        private const string PATH_PREFIX = "$/mocs/";
        public MocImageService(LiteRepository repository)
        {
            Db = repository;
        }
        public LiteFileStream WriteStream(string id, string filename) => Db.FileStorage.OpenWrite(id, filename);
        public LiteFileStream ReadStream(string id) => Db.FileStorage.OpenRead(id);
        public LiteFileInfo ReadFileInfo(string id) => Db.FileStorage.FindById(id);
        public bool UploadFile(int mocId, Stream fileStream)
        {
            // TODO: Save image temporarily, process through job
            if (mocId < 1)
                return false;
            var path = PATH_PREFIX + mocId;
            var filename = mocId.ToString();
            using var image = Image.Load(fileStream);
            image.Mutate(c => c.AutoOrient());
            image.Mutate(c => c.EntropyCrop());
            image.Mutate(c => c.Resize(640, 0));
            using var writeStream = WriteStream(path, filename);
            image.SaveAsJpeg(writeStream);
            return true;
        }
        public async Task<(Stream fileStream, string contentType, string fileDownloadName, string error)> DownloadFile(int mocId)
        {
            if (mocId < 1)
                return (null, null, null, "error - invalid id");
            var path = PATH_PREFIX + mocId;
            var memory = new MemoryStream();
            var fileInfo = ReadFileInfo(path);
            if (fileInfo == null)
                return (null, null, null, "error - no image");
            using var stream = ReadStream(path);
            await stream.CopyToAsync(memory);
            memory.Position = 0;
            return (memory, "image/jpeg", fileInfo.Filename + ".jpg", null);
        }
        public void Dispose() => Db?.Dispose();
    }
}