using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocService: IDisposable
    {
        private LiteRepository db {get;}
        public MocService(LiteRepository repository)
        {
           db = repository;
        }

        public int Create(Moc t) => db.Insert<Moc>(t);
        public Moc Read(int id) => db.Query<Moc>().SingleById(id);
        public IEnumerable<Moc> Read(IEnumerable<int> ids) => ids.Select(Read);
        public IEnumerable<Moc> ReadAll() => db.Query<Moc>().ToEnumerable();
        public bool Update(Moc t) => db.Update<Moc>(t);
        public bool Delete(int id) => db.Delete<Moc>(id);
        public void Dispose()
        {
            db?.Dispose();
        }
    }
}