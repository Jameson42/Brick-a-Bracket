using System;
using System.Collections.Generic;
using System.Linq;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Derby
{
    public class DerbyTournament: ITournamentStrategy
    {
        private const int roundSize = 4;
        private Tournament _tournament; 
        private TournamentService _tournamentService;
        private MocService _mocService;
        private CompetitorService _competitorService;
        public DerbyTournament(TournamentService tournamentService, 
            MocService mocService, CompetitorService competitorService, 
            int tournamentId)
        {
            _tournamentService = tournamentService;
            _mocService = mocService;
            _competitorService = competitorService;
            _tournament = _tournamentService.Read(tournamentId);
        }

        public bool GenerateRounds()
        {
            // Don't overwrite existing data
            if (_tournament.Categories.Any())
                return false;
            // Pull Mocs from MocService
            var mocs = _mocService.Read(_tournament.MocIds).ToList();
            // Create Category items from added Moc categories
            foreach (var id in mocs.Select(m => m.ClassificationId).Distinct())
            {
                var category = new Category(id);
                var classMocIds = mocs
                    .Where(m => m.ClassificationId == id)
                    .Select(m => m._id);
                // Add Moc IDs to Categories
                category.MocIds.AddRange(classMocIds);
                var round = new Round();
                for (int i=0;i<category.MocIds.Count();i++)
                {
                    // Generate Matches in the only Round
                    for(int j=0;j<roundSize;j++)
                    {
                        // Fill matches with MOCs
                    }
                }
                category.Rounds.Add(round);
                _tournament.Categories.Add(category);
            }
            _tournamentService.Update(_tournament);
            return true;
        }
    }
}
