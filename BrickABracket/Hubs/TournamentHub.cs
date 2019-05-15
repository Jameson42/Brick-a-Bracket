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
            var tournament = new Tournament(){
                Name = name,
                TournamentType = type
            };
            return await CreateTournament(tournament);
        }
        private async Task<int> CreateTournament(Tournament t)
        {
            var id = _tournaments.Create(t);
            var tournament = _tournaments.Read(id);
            _runner.Tournament = tournament;
            await SendTournaments();
            return id;
        }
        public async Task GetTournament()
        {
            await Clients.Caller.SendAsync("ReceiveTournament",_runner.Metadata);
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
                await SendTournaments();
            }
        }
        #endregion
        // CRUD Devices
        #region Device CRUD
        public async Task SendDevices() => await Clients.All.SendAsync("ReceiveDevices", _devices.Devices);
        public async Task CreateDevice(string connectionString, string program, string type="NXT", string role="All")
        {
            if (!_devices.Add(connectionString, program, type, role))
                return;
            await SendDevices();
        }
        public async Task GetDevices()
        {
            await Clients.Caller.SendAsync("ReceiveDevices", _devices.Devices);
        }
        public async Task SetDeviceRole(string connectionString, string role)
        {
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
        public async Task SendClassifications() => await Clients.All.SendAsync("ReceiveClassifications",_classes.ReadAll());
        public async Task<int> CreateClassification(string name)
        {
            var result = _classes.Create(new Classification(){Name = name});
            if(result>0)
                await SendClassifications();
            return result;
        }
        public async Task GetClassifications()
        {
            await Clients.Caller.SendAsync("ReceiveClassifications",_classes.ReadAll());
        }
        public async Task UpdateClassification(Classification classification)
        {
            if(_classes.Update(classification))
                await SendClassifications();
        }
        public async Task DeleteClassification(int id)
        {
            if(_classes.Delete(id))
                await SendClassifications();
        }
        #endregion
        // CRUD Competitors
        #region Competitor CRUD
        public async Task SendCompetitors() => await Clients.All.SendAsync("ReceiveCompetitors",_competitors.ReadAll());
        public async Task<int> CreateCompetitor(Competitor competitor)
        {
            var result = _competitors.Create(competitor);
            if(result>0)
                await SendCompetitors();
            return result;
        }
        public async Task GetCompetitors()
        {
            await Clients.Caller.SendAsync("ReceiveCompetitors",_competitors.ReadAll());
        }
        public async Task UpdateCompetitor(Competitor competitor)
        {
            if(_competitors.Update(competitor))
                await SendCompetitors();
        }
        public async Task DeleteCompetitor(int id)
        {
            if(_competitors.Delete(id))
                await SendCompetitors();
        }
        #endregion
        // CRUD Mocs
        #region MOC CRUD
        public async Task SendMocs() => await Clients.All.SendAsync("ReceiveMocs", _mocs.ReadAll());
        public async Task<int> CreateMoc(Moc moc)
        {
            var result = _mocs.Create(moc);
            if(result>0)
                await SendMocs();
            return result;
        }
        public async Task GetMocs()
        {
            await Clients.Caller.SendAsync("ReceiveMocs",_mocs.ReadAll());
        }
        public async Task UpdateMoc(Moc moc)
        {
            if(_mocs.Update(moc))
                await SendMocs();
        }
        public async Task DeleteMoc(int id)
        {
            if(_mocs.Delete(id))
                await SendMocs();
        }
        #endregion
        // Tournament running
        #region Run Tournaments
        public async Task SetActiveTournament(int id)
        {
            _runner.Tournament = _tournaments.Read(id);
            await SendTournaments();
        }
        public async Task AddMocToTournament(int id)
        {
            var mocs = _runner.Tournament.MocIds;
            if (!mocs.Contains(id))
            {
                mocs.Add(id);
                _tournaments.Update(_runner.Tournament);
                await SendTournaments();
            }
        }
        public async Task GenerateCategories()
        {
            _runner.GenerateCategories();
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
        public async Task NextMatch()
        {
            if (_runner.NextMatch())
                await SendTournaments();
        }
        public void ReadyMatch() => _runner.ReadyMatch();
        public void StartMatch() => _runner.StartMatch();
        public void StartTimedMatch(long milliseconds) => _runner.StartTimedMatch(milliseconds);
        public void StopMatch() => _runner.StopMatch();
        #endregion
    }
}