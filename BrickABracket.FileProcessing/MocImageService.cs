using System;
using System.IO;
using System.Threading.Tasks;
using BrickABracket.Core.ORM;
using BrickABracket.Models.Base;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BrickABracket.FileProcessing
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocImageService : IDisposable
    {
        private readonly Repository<MocImage> _repository;
        public MocImageService(Repository<MocImage> repository)
        {
            _repository = repository;
        }

        public async Task<bool> UploadFile(int mocId, Stream fileStream)
        {
            if (mocId < 1)
                return false;
            using var image = Image.Load(fileStream);
            var mocImage = new MocImage
            {
                File = image.ToByteArray(),
                _id = mocId,
                Name = $"{mocId}.jpg",
                Type = "image/jpeg"
            };
            await _repository.CreateOrUpdateAsync(mocImage);
            return true;
        }
        public async Task<bool> ProcessImage(int mocId)
        {
            var mocImage = await DownloadFile(mocId);
            if (mocImage == null)
            {
                throw new ImageProcessingException("Error retrieving file");
            }
            using var image = Image.Load(mocImage.File);
            image.Mutate(c => c.AutoOrient());
            image.Mutate(c => c.EntropyCrop());
            image.Mutate(c => c.Resize(640, 0));
            mocImage.File = image.ToByteArray();
            return await _repository.UpdateAsync(mocImage);
        }
        public async Task<MocImage> DownloadFile(int mocId) =>
            await _repository.ReadAsync(mocId);
        public void Dispose() => _repository?.Dispose();
    }

    internal static class ImageExtensions
    {
        internal static byte[] ToByteArray(this Image image)
        {
            using var writeStream = new MemoryStream();
            image.SaveAsJpeg(writeStream);
            return writeStream.ToArray();
        }
    }
}