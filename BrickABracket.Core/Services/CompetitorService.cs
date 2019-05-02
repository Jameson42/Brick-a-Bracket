using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class CompetitorService: IDisposable
    {
        private LiteRepository db {get;}
        public CompetitorService(LiteRepository repository)
        {
           db = repository;
        }

        public int Create(Competitor t) => db.Insert<Competitor>(t);
        public Competitor Read(int id) => db.Query<Competitor>().SingleById(id);
        public IEnumerable<Competitor> Read(IEnumerable<int> ids) => ids.Select(Read);
        public IEnumerable<Competitor> ReadAll() => db.Query<Competitor>().ToEnumerable();
        public bool Update(Competitor t) => db.Update<Competitor>(t);
        public bool Delete(int id) => db.Delete<Competitor>(id);
        public void Dispose()
        {
            db?.Dispose();
        }
    }
}