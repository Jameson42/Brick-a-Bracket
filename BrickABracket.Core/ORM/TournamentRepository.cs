using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrickABracket.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace BrickABracket.Core.ORM
{
    public class TournamentRepository : Repository<Tournament>
    {
        internal readonly IQueryable<Tournament>  fullTournaments;
        public TournamentRepository(DbContext context) : base(context)
        {
            fullTournaments = _set
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Rounds)
                        .ThenInclude(r => r.Matches)
                            .ThenInclude(m => m.Results)
                                .ThenInclude(mr => mr.Scores)
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Rounds)
                        .ThenInclude(r => r.Standings)
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Standings);
        }
        public override Tournament Read(int id) => 
            fullTournaments.Where(t => t._id == id).FirstOrDefault();
        public override async Task<Tournament> ReadAsync(int id) => 
            await fullTournaments.Where(t => t._id == id).FirstOrDefaultAsync();
        public override IEnumerable<Tournament> Read(IEnumerable<int> ids) =>
            fullTournaments.Where(t => ids.Contains(t._id));
        public override IAsyncEnumerable<Tournament> ReadAsync(IEnumerable<int> ids) =>
            fullTournaments.Where(t => ids.Contains(t._id)).AsAsyncEnumerable();
        public override IEnumerable<Tournament> ReadAll() => 
            fullTournaments.AsEnumerable();
        public override IAsyncEnumerable<Tournament> ReadAllAsync() => 
            fullTournaments.AsAsyncEnumerable();
    }
}