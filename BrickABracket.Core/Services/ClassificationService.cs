using System;
using System.Collections.Generic;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class ClassificationService : IDisposable
    {
        private LiteRepository Db { get; }
        public ClassificationService(LiteRepository repository)
        {
            Db = repository;
        }

        public int Create(Classification t) => Db.Insert(t);
        public Classification Read(int id) => Db.Query<Classification>().SingleById(id);
        public IEnumerable<Classification> ReadAll() => Db.Query<Classification>().ToEnumerable();
        public bool Update(Classification t) => Db.Update(t);
        public bool Delete(int id) => Db.Delete<Classification>(id);
        public void Dispose() => Db?.Dispose();
    }
}