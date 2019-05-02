using System;
using System.Collections.Generic;
using Autofac.Features.Metadata;
using LiteDB;
using BrickABracket.Models.Interfaces;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    public class TournamentService: IDisposable
    {
        private LiteRepository db {get;}
        public TournamentService(LiteRepository repository)
        {
            db = repository;
        }

        public int Create(Tournament t) => db.Insert<Tournament>(t);
        public Tournament Read(int id) => db.Query<Tournament>().SingleById(id);
        public IEnumerable<Tournament> ReadAll() => db.Query<Tournament>().ToEnumerable();
        public IEnumerable<TournamentSummary> ReadAllSummaries() => db.Database
            .GetCollection<TournamentSummary>(typeof(Tournament).Name)
            .FindAll();

        public bool Update(Tournament t) => db.Update<Tournament>(t);
        public bool Delete(int id) => db.Delete<Tournament>(id);
        public void Dispose()
        {
            db?.Dispose();
        }
    }
}