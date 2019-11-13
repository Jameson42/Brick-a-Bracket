using System;
using System.Collections.Generic;
using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    public class TournamentService : IDisposable
    {
        private LiteRepository Db { get; }
        public TournamentService(LiteRepository repository)
        {
            Db = repository;
        }

        public int Create(Tournament t) => Db.Insert(t);
        public Tournament Read(int id) => Db.Query<Tournament>().SingleById(id);
        public IEnumerable<Tournament> ReadAll() => Db.Query<Tournament>().ToEnumerable();
        public IEnumerable<TournamentSummary> ReadAllSummaries() => Db.Database
            .GetCollection<TournamentSummary>(typeof(Tournament).Name)
            .FindAll();
        public bool Update(Tournament t) => Db.Update(t);
        public bool Delete(int id) => Db.Delete<Tournament>(id);
        public void Dispose() => Db?.Dispose();
    }
}