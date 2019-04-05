using System.Collections.Generic;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class CompetitorManager
    {
        private LiteRepository db {get;}
        public CompetitorManager(LiteRepository repository)
        {
           db = repository;
        }

        public int Create(Competitor t) => db.Insert<Competitor>(t);
        public Competitor Read(int id) => db.Query<Competitor>().SingleById(id);
        public IEnumerable<Competitor> ReadAll() => db.Query<Competitor>().ToEnumerable();
        public bool Update(Competitor t) => db.Update<Competitor>(t);
        public bool Delete(int id) => db.Delete<Competitor>(id);
    }
}