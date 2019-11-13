using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocService : IDisposable
    {
        private LiteRepository Db { get; }
        public MocService(LiteRepository repository)
        {
            Db = repository;
        }

        public int Create(Moc t) => Db.Insert(t);
        public Moc Read(int id) => Db.Query<Moc>().SingleById(id);
        public IEnumerable<Moc> Read(IEnumerable<int> ids) => ids.Select(Read);
        public IEnumerable<Moc> ReadAll() => Db.Query<Moc>().ToEnumerable();
        public bool Update(Moc t) => Db.Update(t);
        public bool Delete(int id) => Db.Delete<Moc>(id);
        public void Dispose() => Db?.Dispose();
    }
}