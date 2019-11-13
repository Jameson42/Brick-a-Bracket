using System;
using LiteDB;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocImageService : IDisposable
    {
        private LiteRepository Db { get; }
        public MocImageService(LiteRepository repository)
        {
            Db = repository;
        }
        public LiteFileStream WriteStream(string id, string filename) => Db.FileStorage.OpenWrite(id, filename);
        public LiteFileStream ReadStream(string id) => Db.FileStorage.OpenRead(id);
        public LiteFileInfo ReadFileInfo(string id) => Db.FileStorage.FindById(id);
        public void Dispose() => Db?.Dispose();
    }
}