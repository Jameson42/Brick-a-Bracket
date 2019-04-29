using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Reactive.Linq;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using BrickABracket.Services;

namespace BrickABracket.Hubs
{
    public class TournamentHub : Hub
    {
        #region Services
        private ClassificationService _classes;
        private CompetitorService _competitors;
        private DeviceService _devices;
        private MocService _mocs;
        private TournamentService _tournaments;
        private TournamentRunner _runner;
        #endregion
        public TournamentHub(ClassificationService classes,
            CompetitorService competitors,
            DeviceService devices,
            MocService mocs,
            TournamentService tournaments, 
            TournamentRunner runner,
            MatchWatcher watcher)
        {
            _classes = classes;
            _competitors = competitors;
            _devices = devices;
            _mocs = mocs;
            _tournaments = tournaments;
            _runner = runner;
            // Don't need to use anything in MatchWatcher,
            // only including it here to ensure
            // an instance gets created.
        }
        // CRUD Tournaments (metadata only? How do I keep frontend from editing Matches, Rounds, etc?)
        #region Tournament CRUD
        //Creating or updating a tournament will always activate it.
        public async Task SendTournament()
        {
            await Clients.All.SendAsync("ReceiveTournament", _runner.Metadata);
        }
        public async Task CreateTournament(Tournament t)
        {
            var id = _tournaments.Create(t);
            await Clients.All.SendAsync("ReceiveTournamentSummaries", _tournaments.ReadAllSummaries());
            var tournament = _tournaments.Read(id);
            _runner.Tournament = tournament;
            await SendTournament();
        }
        public async Task GetTournament()
        {
            await Clients.Caller.SendAsync("ReceiveTournament",_runner.Tournament);
        }
        public async Task UpdateTournament(Tournament t)
        {
            if (_tournaments.Update(t))
            {
                _runner.Tournament = _tournaments.Read(t._id);
                await SendTournament();
            }
        }
        public async Task DeleteTournament(int id)
        {
            if (_tournaments.Delete(id))
            {
                _runner.Tournament = null;
                await SendTournament();
            }
        }
        #endregion
        // CRUD Devices
        #region Device CRUD
        public async Task CreateDevice(string connectionString, string type="NXT", string role="All")
        {
            if (!_devices.Add(connectionString, type, role))
                return;
            await Clients.All.SendAsync("ReceiveDevices", _devices.Devices);
        }
        public async Task GetDevices()
        {
            await Clients.Caller.SendAsync("ReceiveDevices", _devices.Devices);
        }
        public async Task SetDeviceRole(string connectionString, string role)
        {
            if (!_devices.SetRole(connectionString, role))
                return;
            await Clients.All.SendAsync("ReceiveDevices", _devices.Devices);
        }
        public async Task DeleteDevice(string connectionString)
        {
            if (!_devices.Remove(connectionString))
                return;
            await Clients.All.SendAsync("ReceiveDevices", _devices.Devices);
        }
        #endregion
        // CRUD Classifications
        #region Classification CRUD
        public async Task CreateClassification(Classification classification)
        {
            if(_classes.Create(classification)>0)
                await Clients.All.SendAsync("ReceiveClassifications",_classes.ReadAll());
        }
        public async Task GetClassifications()
        {
            await Clients.Caller.SendAsync("ReceiveClassifications",_classes.ReadAll());
        }
        public async Task UpdateClassification(Classification classification)
        {
            if(_classes.Update(classification))
                await Clients.All.SendAsync("ReceiveClassifications",_classes.ReadAll());
        }
        public async Task DeleteClassification(int id)
        {
            if(_classes.Delete(id))
                await Clients.All.SendAsync("ReceiveClassifications",_classes.ReadAll());
        }
        #endregion
        // CRUD Competitors
        #region Competitor CRUD
        public async Task CreateCompetitor(Competitor competitor)
        {
            if(_competitors.Create(competitor)>0)
                await Clients.All.SendAsync("ReceiveCompetitors",_competitors.ReadAll());
        }
        public async Task GetCompetitors()
        {
            await Clients.Caller.SendAsync("ReceiveCompetitors",_competitors.ReadAll());
        }
        public async Task UpdateCompetitor(Competitor competitor)
        {
            if(_competitors.Update(competitor))
                await Clients.All.SendAsync("ReceiveCompetitors",_competitors.ReadAll());
        }
        public async Task DeleteCompetitor(int id)
        {
            if(_competitors.Delete(id))
                await Clients.All.SendAsync("ReceiveCompetitors",_competitors.ReadAll());
        }
        #endregion
        // CRUD Mocs
        #region MOC CRUD
        public async Task CreateMoc(Moc moc)
        {
            if(_mocs.Create(moc)>0)
                await Clients.All.SendAsync("ReceiveMocs",_mocs.ReadAll());
        }
        public async Task GetMocs()
        {
            await Clients.Caller.SendAsync("ReceiveMocs",_mocs.ReadAll());
        }
        public async Task UpdateMoc(Moc moc)
        {
            if(_mocs.Update(moc))
                await Clients.All.SendAsync("ReceiveMocs",_mocs.ReadAll());
        }
        public async Task DeleteMoc(int id)
        {
            if(_mocs.Delete(id))
                await Clients.All.SendAsync("ReceiveMocs",_mocs.ReadAll());
        }
        #endregion
        // Tournament running
        #region Run Tournaments
        public async Task SetCategoryIndex(int i)
        {
            _runner.CategoryIndex = i;
            await SendTournament();
        }
        public async Task SetRoundIndex(int i)
        {
            _runner.RoundIndex = i;
            await SendTournament();
        }
        public async Task SetMatchIndex(int i)
        {
            _runner.MatchIndex = i;
            await SendTournament();
        }
        public async Task NextMatch()
        {
            if (_runner.NextMatch())
                await SendTournament();
        }
        public async Task StartMatch()
        {
            _runner.StartMatch();
        }
        public async Task StopMatch()
        {
            _runner.StopMatch();
        }
        #endregion
    }
}