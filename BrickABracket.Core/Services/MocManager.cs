using System.Collections.Generic;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class MocManager
    {
        private LiteRepository db {get;}
        public MocManager(LiteRepository repository)
        {
           db = repository;
        }

        public int Create(Moc t) => db.Insert<Moc>(t);
        public Moc Read(int id) => db.Query<Moc>().SingleById(id);
        public IEnumerable<Moc> ReadAll() => db.Query<Moc>().ToEnumerable();
        public bool Update(Moc t) => db.Update<Moc>(t);
        public bool Delete(int id) => db.Delete<Moc>(id);
    }
}