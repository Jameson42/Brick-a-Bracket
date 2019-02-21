using System;
using System.Collections.Generic;
using LiteDB;

namespace BrickABracket.Core.ORM
{
    public class Repository: IDisposable
    {
        private LiteDatabase _db;
        public Repository(string connectionString) => _db = new LiteDatabase(connectionString);
        public T Get<T>(int id) => _db.GetCollection<T>(typeof(T).Name).FindById(id);
        public IEnumerable<T> GetAll<T>() => _db.GetCollection<T>(typeof(T).Name).FindAll();
        public BsonValue Insert<T>(T item) => _db.GetCollection<T>(typeof(T).Name).Insert(item);
        public bool Update<T>(T item) => _db.GetCollection<T>(typeof(T).Name).Upsert(item);
        public bool Remove<T>(int id) => _db.GetCollection<T>(typeof(T).Name).Delete(id);
        public void Dispose() => _db.Dispose();
    }
}