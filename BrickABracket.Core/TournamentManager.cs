using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using BrickABracket.Models.Interfaces;
using BrickABracket.Core.ORM;

namespace BrickABracket.Core
{
    public class TournamentManager
    {
        private IEnumerable<Meta<Func<ITournament>>> _tournamentFactories;
        private Func<Repository> _repositoryFactory;
        public TournamentManager(IEnumerable<Meta<Func<ITournament>>> tournamentFactories, Func<Repository> repositoryFactory)
        {
            _tournamentFactories = tournamentFactories;
            _repositoryFactory = repositoryFactory;
        }

        private ITournament Create(string typeKey)
        {
            return _tournamentFactories
                .FirstOrDefault(tf => (string)tf.Metadata["Type"] == typeKey?.ToLower())
                .Value.Invoke();
        }

        private ITournament Get(int id)
        {
            using (var db = _repositoryFactory())
            {
                var tournament = db.Get<ITournament>(id);
                var type = Create(tournament.Type).GetType();
                return (ITournament)Convert.ChangeType(tournament, type);
                // This seems clunky... what's a better way? 
                // Maybe change to constructor on each tournament type which takes an ITournament
            }
        }
    }
}