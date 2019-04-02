using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using BrickABracket.Models.Interfaces;
using BrickABracket.Core.ORM;
using BrickABracket.Models.Base;

namespace BrickABracket.Core
{
    // Runs tournaments, retrieving and saving to repository as necessary.
    public class TournamentManager
    {
        private IEnumerable<Meta<ITournamentStrategy>> _tournamentStrategies;
        // Create a new repository reference on each get/save action
        private Func<Repository> _repositoryFactory;
        public TournamentManager(IEnumerable<Meta<ITournamentStrategy>> tournamentStrategies, Func<Repository> repositoryFactory)
        {
            _tournamentStrategies = tournamentStrategies;
            _repositoryFactory = repositoryFactory;
        }

        private Tournament GetTournament(int id)
        {
            using (var db = _repositoryFactory())
                return db.Get<Tournament>(id);
        }

        private bool UpdateTournament(Tournament t)
        {
            using (var db = _repositoryFactory())
                return db.Update<Tournament>(t);
        } 
    }
}