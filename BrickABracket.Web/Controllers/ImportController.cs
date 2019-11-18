using BrickABracket.Core.Services;
using BrickABracket.FileProcessing;
using BrickABracket.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BrickABracket.Controllers
{
    [Route("api/import")]
    public class ImportController : Controller
    {
        private readonly CompetitorService _competitors;
        private readonly CompetitorImportService _competitorImport;
        private readonly IHubContext<TournamentHub> _hub;
        public ImportController(CompetitorService competitors,
            CompetitorImportService competitorImport,
            IHubContext<TournamentHub> hub)
        {
            _competitors = competitors;
            _competitorImport = competitorImport;
            _hub = hub;
        }
        [HttpPost("{competitors}")]
        public async Task<IActionResult> UploadCompetitors(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return Content("error");
            using var fileStream = file.OpenReadStream();
            _competitorImport.UploadCompetitors(fileStream);
            await _hub.Clients.All.SendAsync("ReceiveCompetitors", _competitors.ReadAll());
            return Content("success");
        }
    }
}