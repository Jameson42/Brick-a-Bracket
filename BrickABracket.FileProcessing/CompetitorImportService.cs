using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using FileHelpers;

namespace BrickABracket.FileProcessing
{
    public class CompetitorImportService
    {
        private readonly CompetitorService _competitors;
        public CompetitorImportService(CompetitorService competitors)
        {
            _competitors = competitors;
        }

        public async Task UploadCompetitors(Stream fileStream)
        {
            IEnumerable<CompetitorMapping> results;

            var engine = new FileHelperEngine<CompetitorMapping>();
            using (var fileStreamReader = new StreamReader(fileStream))
            {
                results = engine
                    .ReadStream(fileStreamReader)
                    .Where(r => !string.IsNullOrWhiteSpace(r.Name))
                    .DistinctBy(r => r.Name);
            }

            var competitors = _competitors.ReadAll();

            foreach (var result in results)
            {
                if (!competitors.Any(c => c.Name == result.Name))
                {
                    _competitors.Create(new Competitor()
                    {
                        Name = result.Name
                    });
                }
            }
        }
    }

    [DelimitedRecord("\t")]
    public class CompetitorMapping
    {
        public string Name;
        [FieldOptional]
        public string Location;
        [FieldOptional]
        public string LUG;
    }

    public static class LinqAddons
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }

}