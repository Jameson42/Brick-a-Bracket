using System.Collections.Generic;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class PatternService
    {
        private LiteRepository db {get;}
        public PatternService(LiteRepository repository)
        {
           db = repository;
        }

        public int Create(TournamentPattern t) => db.Insert<TournamentPattern>(t);
        public TournamentPattern Read(string id) => db.Query<TournamentPattern>().SingleById(id);
        public IEnumerable<TournamentPattern> ReadAll() => db.Query<TournamentPattern>().ToEnumerable();
        public bool Update(TournamentPattern t) => db.Update<TournamentPattern>(t);
        public bool Upsert (TournamentPattern t) => db.Upsert<TournamentPattern>(t);
        public bool Delete(string id) => db.Delete<TournamentPattern>(id);
    }
}