using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using BrickABracket.Models.Interfaces;
using BrickABracket.Core.ORM;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manages competitors, retrieving and saving to repository as necessary.
    public class CompetitorManager
    {
        // Create a new repository reference on each get/save action
        private Func<Repository> _repositoryFactory;
        public CompetitorManager(Func<Repository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public int Create(Competitor t)
        {
            using (var db = _repositoryFactory())
                return db.Insert<Competitor>(t);
        }

        public Competitor Read(int id)
        {
            using (var db = _repositoryFactory())
                return db.Get<Competitor>(id);
        }

        public IEnumerable<Competitor> ReadAll()
        {
            using (var db = _repositoryFactory())
                return db.GetAll<Competitor>();
        }

        public bool Update(Competitor t)
        {
            using (var db = _repositoryFactory())
                return db.Update<Competitor>(t);
        }

        public bool Delete(int id)
        {
            using (var db = _repositoryFactory())
                return db.Remove<Competitor>(id);
        }
    }
}