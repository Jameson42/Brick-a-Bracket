using System.Collections.Generic;
using Autofac.Features.Metadata;
using LiteDB;
using BrickABracket.Models.Interfaces;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Runs tournaments, retrieving and saving to repository as necessary.
    public class TournamentService
    {
        private IEnumerable<Meta<ITournamentStrategy>> _tournamentStrategies;
        private LiteRepository db {get;}
        private ActivesService _actives {get;}
        public TournamentService(IEnumerable<Meta<ITournamentStrategy>> tournamentStrategies, 
            LiteRepository repository, ActivesService actives)
        {
            _tournamentStrategies = tournamentStrategies;
            db = repository;
            _actives = actives;
        }

        public int Create(Tournament t) => db.Insert<Tournament>(t);
        public Tournament Read(int id) => db.Query<Tournament>().SingleById(id);
        public IEnumerable<Tournament> ReadAll() => db.Query<Tournament>().ToEnumerable();
        public IEnumerable<TournamentSummary> ReadAllSummaries() => db.Database
            .GetCollection<TournamentSummary>(typeof(Tournament).Name)
            .FindAll();

        public bool Update(Tournament t) => db.Update<Tournament>(t);
        public bool Delete(int id) => db.Delete<Tournament>(id);

        // TODO: Add methods to run tournaments through strategies
    }
}