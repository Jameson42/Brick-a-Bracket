using BrickABracket.Core.ORM;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BrickABracket.Web.Hubs
{
    public class TournamentHub : Hub
    {
        #region Services
        private readonly Repository<Classification> _classes;
        private readonly Repository<Competitor> _competitors;
        private readonly Repository<Moc> _mocs;
        private readonly Repository<Tournament> _tournaments;
        private readonly DeviceService _devices;
        private readonly TournamentRunner _runner;
        private readonly ScoreTracker _scores;
        private readonly StatusTracker _statuses;
        #endregion
        public TournamentHub(Repository<Classification> classes,
            Repository<Competitor> competitors,
            DeviceService devices,
            Repository<Moc> mocs,
            Repository<Tournament> tournaments,
            TournamentRunner runner,
            ScoreTracker scores,
            StatusTracker statuses)
        {
            _classes = classes;
            _competitors = competitors;
            _devices = devices;
            _mocs = mocs;
            _tournaments = tournaments;
            _runner = runner;
            _scores = scores;
            _statuses = statuses;
        }

        // CRUD Tournaments
        #region Tournament CRUD
        //Creating or updating a tournament will always activate it.
        public async Task SendTournaments()
        {
            await Task.WhenAll(
                Clients.All.SendAsync("ReceiveTournament", _runner.Metadata),
                Clients.All.SendAsync("ReceiveTournamentSummaries", _tournaments.ReadAllSummaries())
            );
        }
        public async Task<int> CreateTournament(string name, string type)
        {
            var tournament = new Tournament()
            {
                Name = name,
                TournamentType = type
            };
            return await CreateTournament(tournament);
        }
        private async Task<int> CreateTournament(Tournament t)
        {
            var id = await _tournaments.CreateAsync(t);
            await SetActiveTournament(id);
            return id;
        }
        public async Task GetTournament() =>
            await Clients.Caller.SendAsync("ReceiveTournament", _runner.Metadata);
        public async Task GetTournamentSummaries() =>
            await Clients.Caller.SendAsync("ReceiveTournamentSummaries", _tournaments.ReadAllSummaries());
        public async Task UpdateTournament(Tournament t)
        {
            var fullTournament = await _tournaments.ReadAsync(t._id);
            // Do not allow frontend to change sub-classes directly
            fullTournament.Name = t.Name;
            fullTournament.TournamentType = t.TournamentType;
            fullTournament.MocIds = t.MocIds;
            if (await _tournaments.UpdateAsync(fullTournament))
            {
                await SetActiveTournament(t._id);
            }
        }
        public async Task DeleteTournament(int id)
        {
            foreach (var mId in (await _tournaments.ReadAsync(id))?.MocIds)
            {
                await _mocs.DeleteAsync(mId);
            }
            if (await _tournaments.DeleteAsync(id))
            {
                _runner.Tournament = null;
                await SendTournaments();
            }
        }
        #endregion
        // CRUD Devices
        #region Device CRUD
        public async Task SendDevices() =>
            await Clients.All.SendAsync("ReceiveDevices", _devices.Devices);
        public async Task CreateDevice(string type, string connectionString, string roleString = "0", string program = "")
        {
            if (!int.TryParse(roleString, out int role))
                return;
            if (_devices.Add(type, connectionString, role, program) is null)
                return;
            await SendDevices();
        }
        public async Task CreateAllDevices()
        {
            if (_devices.TryAddAll().Count() > 0)
                await SendDevices();
        }
        public async Task GetDevices() =>
            await Clients.Caller.SendAsync("ReceiveDevices", _devices.Devices);
        public async Task GetDeviceOptions() =>
            await Clients.Caller.SendAsync("ReceiveDeviceOptions", _devices.GetDeviceOptions());
        public async Task SetDeviceRole(string connectionString, string roleString)
        {
            if (!int.TryParse(roleString, out int role))
                return;
            if (!_devices.SetRole(connectionString, role))
                return;
            await SendDevices();
        }
        public async Task SetDeviceProgram(string connectionString, string program)
        {
            if (!_devices.SetProgram(connectionString, program))
                return;
            await SendDevices();
        }
        public async Task DeleteDevice(string connectionString)
        {
            if (!_devices.Remove(connectionString))
                return;
            await SendDevices();
        }
        #endregion
        // CRUD Classifications
        #region Classification CRUD
        public async Task SendClassifications() =>
            await Clients.All.SendAsync("ReceiveClassifications", _classes.ReadAll());
        public async Task<int> CreateClassification(string name)
        {
            var result = await _classes.CreateAsync(new Classification() { Name = name });
            if (result > 0)
                await SendClassifications();
            return result;
        }
        public async Task GetClassifications()
        {
            await Clients.Caller.SendAsync("ReceiveClassifications", _classes.ReadAll());
        }
        public async Task UpdateClassification(Classification classification)
        {
            if (await _classes.UpdateAsync(classification))
                await SendClassifications();
        }
        public async Task DeleteClassification(int id)
        {
            if (await _classes.DeleteAsync(id))
                await SendClassifications();
        }
        #endregion
        // CRUD Competitors
        #region Competitor CRUD
        public async Task SendCompetitors() =>
            await Clients.All.SendAsync("ReceiveCompetitors", _competitors.ReadAll());
        public async Task<int> CreateCompetitor(Competitor competitor)
        {
            var result = await _competitors.CreateAsync(competitor);
            if (result > 0)
                await SendCompetitors();
            return result;
        }
        public async Task GetCompetitors() =>
            await Clients.Caller.SendAsync("ReceiveCompetitors", _competitors.ReadAll());
        public async Task UpdateCompetitor(Competitor competitor)
        {
            if (await _competitors.UpdateAsync(competitor))
                await SendCompetitors();
        }
        public async Task DeleteCompetitor(int id)
        {
            if (await _competitors.DeleteAsync(id))
                await SendCompetitors();
        }
        #endregion
        // CRUD Mocs
        #region MOC CRUD
        public async Task SendMocs() =>
            await Clients.All.SendAsync("ReceiveMocs", _mocs.ReadAll());
        public async Task<int> CreateMoc(Moc moc)
        {
            var result = await _mocs.CreateAsync(moc);
            if (result > 0)
            {
                await SendMocs();
                await AddMocToTournament(result);
            }
            return result;
        }
        public async Task GetMocs() =>
            await Clients.Caller.SendAsync("ReceiveMocs", _mocs.ReadAll());
        public async Task UpdateMoc(Moc moc)
        {
            if (await _mocs.UpdateAsync(moc))
            {
                await AddMocToTournament(moc._id);
                await SendMocs();
            }
        }
        public async Task DeleteMoc(int id)
        {
            if (await _mocs.DeleteAsync(id))
            {
                await RemoveMocFromTournament(id);
                await SendMocs();
            }
        }
        private async Task AddMocToTournament(int id)
        {
            var mocs = _runner.Tournament.MocIds;
            if (!mocs.Contains(id))
            {
                mocs.Add(id);
                await _tournaments.UpdateAsync(_runner.Tournament);
                await SendTournaments();
            }
        }
        private async Task RemoveMocFromTournament(int id)
        {
            var mocs = _runner.Tournament.MocIds;
            if (mocs.Contains(id))
            {
                mocs.Remove(id);
                await _tournaments.UpdateAsync(_runner.Tournament);
                await SendTournaments();
            }
        }
        #endregion
        // Tournament running
        #region Run Tournaments
        public async Task SetActiveTournament(int id)
        {
            _runner.Tournament = await _tournaments.ReadAsync(id);
            await SendTournaments();
        }
        public async Task GenerateCategories()
        {
            await _runner.GenerateCategories();
            await SendTournaments();
        }
        public async Task SetCategoryIndex(int i)
        {
            _runner.CategoryIndex = i;
            await SendTournaments();
        }
        public async Task SetRoundIndex(int i)
        {
            _runner.RoundIndex = i;
            await SendTournaments();
        }
        public async Task SetMatchIndex(int i)
        {
            _runner.MatchIndex = i;
            await SendTournaments();
        }
        public async Task Runoff(int count)
        {
            await _runner.Runoff(count);
            await SendTournaments();
        }
        public async Task NextMatch()
        {
            await _runner.NextMatch();
            await SendTournaments();
        }
        public void ReadyMatch() => PassStatus(StatusCode.Ready);
        public void StartMatch() => PassStatus(StatusCode.Start);
        public void StartTimedMatch(long milliseconds) =>
            _runner.StartTimedMatch(milliseconds);
        public void StopMatch() => PassStatus(StatusCode.Stop);
        public async Task DeleteCurrentMatch()
        {
            await _runner.DeleteCurrentMatch();
            await SendTournaments();
        }
        public void PassScore(int player, double time) =>
            PassScore(new Score(player, time));
        private void PassScore(Score score) =>
            _scores.PassScore(score);
        public void PassStatus(string status) =>
            PassStatus(status.ToStatus());
        private void PassStatus(Status status) =>
            _statuses.PassStatus(status);
        private void PassStatus(StatusCode code) =>
            PassStatus(Status(code));
        private static Status Status(StatusCode code) =>
            new Status(code, typeof(TournamentHub), "Tournament Hub");
        #endregion
    }
}