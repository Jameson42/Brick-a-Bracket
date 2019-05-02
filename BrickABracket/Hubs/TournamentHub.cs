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
            await Task.WhenAll(
                Clients.All.SendAsync("ReceiveTournament", _runner.Metadata),
                Clients.All.SendAsync("ReceiveTournamentSummaries", _tournaments.ReadAllSummaries())
            );
        } 
        public async Task CreateTournament(string name, string type)
        {
            var tournament = new Tournament(){
                Name = name,
                TournamentType = type
            };
            await CreateTournament(tournament);
        }
        private async Task CreateTournament(Tournament t)
        {
            var id = _tournaments.Create(t);
            var tournament = _tournaments.Read(id);
            _runner.Tournament = tournament;
            await SendTournament();
        }
        public async Task SetActiveTournament(int id)
        {
            _runner.Tournament = _tournaments.Read(id);
            await SendTournament();
        }
        public async Task GetTournament()
        {
            await Clients.Caller.SendAsync("ReceiveTournament",_runner.Tournament);
        }
        public async Task GetTournamentSummaries()
        {
            await Clients.Caller.SendAsync("ReceiveTournamentSummaries", _tournaments.ReadAllSummaries());
        }
        public async Task UpdateTournament(Tournament t)
        {
            var fullTournament = _tournaments.Read(t._id);
            // Do not allow frontend to change sub-classes directly
            fullTournament.Name = t.Name;
            fullTournament.TournamentType = t.TournamentType;
            fullTournament.MocIds = t.MocIds;
            if (_tournaments.Update(fullTournament))
            {
                await SetActiveTournament(t._id);
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
        public async Task CreateDevice(string connectionString, string program, string type="NXT", string role="All")
        {
            if (!_devices.Add(connectionString, program, type, role))
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
        public async Task SetDeviceProgram(string connectionString, string program)
        {
            if (!_devices.SetProgram(connectionString, program))
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
        public async Task GenerateCategories()
        {
            _runner.GenerateCategories();
            await SendTournament();
        }
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
        public async Task ReadyMatch()
        {
            _runner.ReadyMatch();
        }
        public async Task StartMatch()
        {
            _runner.StartMatch();
        }
        public async Task StartTimedMatch(long milliseconds)
        {
            _runner.StartTimedMatch(milliseconds);
        }
        public async Task StopMatch()
        {
            _runner.StopMatch();
        }
        #endregion
    }
}