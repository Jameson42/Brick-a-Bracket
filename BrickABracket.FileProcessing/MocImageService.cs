using System;
using System.IO;
using System.Threading.Tasks;
using LiteDB;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BrickABracket.FileProcessing
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocImageService : IDisposable
    {
        private LiteRepository Db { get; }
        private const string PATH_PREFIX = "$/mocs/";
        private string Path(int mocId) => PATH_PREFIX + mocId;
        private string Filename(int mocId) => mocId.ToString();
        public MocImageService(LiteRepository repository)
        {
            Db = repository;
        }
        private LiteFileStream WriteStream(int mocId) => WriteStream(Path(mocId), Filename(mocId));
        private LiteFileStream WriteStream(string path, string filename) => Db.FileStorage.OpenWrite(path, filename);
        private LiteFileStream ReadStream(int mocId) => ReadStream(Path(mocId));
        private LiteFileStream ReadStream(string path) => Db.FileStorage.OpenRead(path);
        private LiteFileInfo ReadFileInfo(int mocId) => ReadFileInfo(Path(mocId));
        private LiteFileInfo ReadFileInfo(string path) => Db.FileStorage.FindById(path);

        public bool UploadFile(int mocId, Stream fileStream)
        {
            // Save image temporarily, process through job
            if (mocId < 1)
                return false;
            using var image = Image.Load(fileStream);
            using var writeStream = WriteStream(mocId);
            image.SaveAsJpeg(writeStream);
            return true;
        }
        public async Task ProcessImage(int mocId)
        {
            var (readStream, _, _, error) = await DownloadFile(mocId);
            if (error != null)
            {
                throw new ImageProcessingException(error);
            }
            using var image = Image.Load(readStream);
            image.Mutate(c => c.AutoOrient());
            image.Mutate(c => c.EntropyCrop());
            image.Mutate(c => c.Resize(640, 0));
            using var writeStream = WriteStream(mocId);
            image.SaveAsJpeg(writeStream);
        }
        public async Task<(Stream fileStream, string contentType, string fileDownloadName, string error)> DownloadFile(int mocId)
        {
            if (mocId < 1)
                return (null, null, null, "error - invalid id");
            var memory = new MemoryStream();
            var fileInfo = ReadFileInfo(mocId);
            if (fileInfo == null)
                return (null, null, null, "error - no image");
            using var stream = ReadStream(mocId);
            await stream.CopyToAsync(memory);
            memory.Position = 0;
            return (memory, "image/jpeg", fileInfo.Filename + ".jpg", null);
        }
        public void Dispose() => Db?.Dispose();
    }
}