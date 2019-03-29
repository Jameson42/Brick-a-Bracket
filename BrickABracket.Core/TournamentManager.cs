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
        private IEnumerable<Meta<ITournamentStrategy>> _tournamentStrategies;
        private Func<Repository> _repositoryFactory;
        public TournamentManager(IEnumerable<Meta<ITournamentStrategy>> tournamentStrategies, Func<Repository> repositoryFactory)
        {
            _tournamentStrategies = tournamentStrategies;
            _repositoryFactory = repositoryFactory;
        }
    }
}