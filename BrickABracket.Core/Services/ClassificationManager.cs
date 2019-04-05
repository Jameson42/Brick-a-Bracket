using System.Collections.Generic;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class ClassificationManager
    {
        private LiteRepository db {get;}
        public ClassificationManager(LiteRepository repository)
        {
           db = repository;
        }

        public int Create(Classification t) => db.Insert<Classification>(t);
        public Classification Read(int id) => db.Query<Classification>().SingleById(id);
        public IEnumerable<Classification> ReadAll() => db.Query<Classification>().ToEnumerable();
        public bool Update(Classification t) => db.Update<Classification>(t);
        public bool Delete(int id) => db.Delete<Classification>(id);
    }
}