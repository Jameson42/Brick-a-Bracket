using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrickABracket.Core.ORM;
using BrickABracket.Models.Base;
using FileHelpers;

namespace BrickABracket.FileProcessing
{
    public class CompetitorImportService
    {
        private readonly Repository<Competitor> _competitors;
        public CompetitorImportService(Repository<Competitor> competitors)
        {
            _competitors = competitors;
        }

        public void UploadCompetitors(Stream fileStream)
        {
            IEnumerable<CompetitorMapping> results;

            var engine = new FileHelperAsyncEngine<CompetitorMapping>();
            using var fileStreamReader = new StreamReader(fileStream);
            using var stream = engine.BeginReadStream(fileStreamReader);
            results = engine
                .Where(r => !string.IsNullOrWhiteSpace(r.Name))
                .DistinctBy(r => r.Name)
                .Where(r => !_competitors.Exists(r.Name));

            foreach (var result in results)
            {
                _competitors.Create(new Competitor()
                {
                    Name = result.Name
                });
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

    internal static class LinqAddons
    {
        internal static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
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