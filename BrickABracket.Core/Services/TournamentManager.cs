using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using LiteDB;
using BrickABracket.Models.Interfaces;
using BrickABracket.Core.ORM;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Runs tournaments, retrieving and saving to repository as necessary.
    public class TournamentManager
    {
        private IEnumerable<Meta<ITournamentStrategy>> _tournamentStrategies;
        // Create a new repository reference on each get/save action
        private Func<Repository> _repositoryFactory;
        private LiteRepository db {get;}
        public TournamentManager(IEnumerable<Meta<ITournamentStrategy>> tournamentStrategies, LiteRepository repository)
        {
            _tournamentStrategies = tournamentStrategies;
            db = repository;
        }

        public int Create(Tournament t)
        {
            return db.Insert<Tournament>(t);
        }

        public Tournament Read(int id)
        {
            return db.Query<Tournament>()
                .SingleById(id);
        }

        public IEnumerable<Tournament> ReadAll()
        {
            return db.Query<Tournament>()
                .ToEnumerable();
        }

        public bool Update(Tournament t)
        {
            return db.Update<Tournament>(t);
        }

        public bool Delete(int id)
        {
            return db.Delete<Tournament>(id);
        }
    }
}