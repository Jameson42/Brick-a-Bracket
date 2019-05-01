using LiteDB;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocImageService
    {
        private LiteRepository db {get;}
        public MocImageService(LiteRepository repository)
        {
           db = repository;
        }
        public LiteFileStream WriteStream(string id, string filename)
        {
            return  db.FileStorage.OpenWrite(id, filename);
        }
        public LiteFileStream ReadStream(string id)
        {
            return db.FileStorage.OpenRead(id);
        }
        public LiteFileInfo ReadFileInfo(string id)
        {
            return db.FileStorage.FindById(id);
        }
    }
}