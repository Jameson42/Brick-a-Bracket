using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrickABracket.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace BrickABracket.Core.ORM
{
    public static class RepositoryExtensions
    {
        public static bool Exists(this Repository<Competitor> repository, string name)
        {
            return repository._set.Any(c => c.Name == name);
        }
        public static async Task<bool> ExistsAsync(this Repository<Competitor> repository, string name)
        {
            return await repository._set.AnyAsync(c => c.Name == name);
        }
        public static IEnumerable<TournamentSummary> ReadAllSummaries(this Repository<Tournament> repository)
        {
            return repository._context.Set<TournamentSummary>().AsEnumerable();
        }
        public static IAsyncEnumerable<TournamentSummary> ReadAllSummariesAsync(this Repository<Tournament> repository)
        {
            return repository._context.Set<TournamentSummary>().AsAsyncEnumerable();
        }
    }
}