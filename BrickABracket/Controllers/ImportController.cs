using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using FileHelpers;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using BrickABracket.Hubs;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Hosting;

namespace BrickABracket.Controllers
{
    [Route("api/import")]
    public class ImportController : Controller
    {
        private readonly CompetitorService _competitors;
        private readonly IHubContext<TournamentHub> _hub;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ImportController(CompetitorService competitors,
            IHubContext<TournamentHub> hub, IHostingEnvironment environment)
        {
            _competitors = competitors;
            _hub = hub;
            _hostingEnvironment = environment;
        }
        [HttpPost("{competitors}")]
        public async Task<IActionResult> UploadCompetitors(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return Content("error");
            IEnumerable<CompetitorMapping> results;
            
            var engine = new FileHelperEngine<CompetitorMapping>();
            using (var fileStream = new StreamReader(file.OpenReadStream())) {
                results = engine.ReadStream(fileStream).AsEnumerable()
                    .Where(r => !string.IsNullOrWhiteSpace(r.Name))
                    .DistinctBy(r => r.Name);
            }

            var competitors = _competitors.ReadAll();

            foreach(var result in results)
            {
                if (!competitors.Any(c => c.Name == result.Name))
                {
                    _competitors.Create(new Competitor(){
                        Name = result.Name
                    });
                }
            }
            await _hub.Clients.All.SendAsync("ReceiveCompetitors", _competitors.ReadAll());
            return Content("success");
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
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
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