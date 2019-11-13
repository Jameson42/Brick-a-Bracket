using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class CompetitorService : IDisposable
    {
        private LiteRepository Db { get; }
        public CompetitorService(LiteRepository repository)
        {
            Db = repository;
        }

        public int Create(Competitor t) => Db.Insert(t);
        public Competitor Read(int id) => Db.Query<Competitor>().SingleById(id);
        public IEnumerable<Competitor> Read(IEnumerable<int> ids) => ids.Select(Read);
        public IEnumerable<Competitor> ReadAll() => Db.Query<Competitor>().ToEnumerable();
        public bool Update(Competitor t) => Db.Update(t);
        public bool Delete(int id) => Db.Delete<Competitor>(id);
        public void Dispose() => Db?.Dispose();
    }
}