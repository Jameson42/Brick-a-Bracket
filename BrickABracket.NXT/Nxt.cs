using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;
using MonoBrick.NXT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace BrickABracket.NXT
{
    public class Nxt : IDevice, IEquatable<Nxt>
    {
        private static readonly Box STATUS_OUTBOX = Box.Box0;
        private static readonly Box SCORE_OUTBOX = Box.Box1;
        private static readonly Box STATUS_INBOX = Box.Box5;
        private Brick<I2CSensor, I2CSensor, I2CSensor, I2CSensor> _brick;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private IDisposable _followSubscription;
        private readonly Subject<Score> _scores;
        private readonly BehaviorSubject<Status> _statuses;
        public IObservable<Score> Scores { get; private set; }
        public IObservable<Status> Statuses { get; private set; }
        private CancellationTokenSource _messageTokenSource;
        private Task _messageTask;
        private readonly Func<string, Score> _scoreFactory;
        private bool ProgramStarted { get; set; } = false;
        private bool MatchStarted { get; set; } = false;
        private readonly IDeviceRemover _remover;

        public Nxt(string connectionString,
            Func<string, Score> scoreFactory,
            IDeviceRemover remover)
        {
            Connection = connectionString;
            _scoreFactory = scoreFactory;
            _scores = new Subject<Score>();
            _statuses = new BehaviorSubject<Status>(Status(StatusCode.Unknown));
            _remover = remover;
            Scores = _scores.AsObservable();
            Statuses = _statuses.AsObservable();

        }

        public static string[] Ports => MonoBrick.Bluetooth<Command, Reply>
            .GetPortNames()
            .Append("USB")
            .ToArray();

        public bool Connected
        {
            get
            {
                var connectTask = Connect();
                connectTask.Wait();
                return connectTask.Result;
            }
        }

        public string Connection { get; private set; }
        public ushort BatteryLevel
        {
            get
            {
                using (semaphore.Lock())
                    try { return _brick?.GetBatteryLevel() ?? 0; }
                    catch { return 0; }
            }
        }
        public string BrickName
        {
            get
            {
                using (semaphore.Lock())
                    try { return _brick?.GetBrickName() ?? ""; }
                    catch { return ""; }
            }
        }
        public string Program { get; set; }

        public IEnumerable<string> GetPrograms()
        {
            BrickFile[] files;
            using (semaphore.Lock())
                files = _brick.FileSystem.FileList();
            return files
                .Where(bf => bf.FileType == MonoBrick.FileType.Program)
                .Select(bf => bf.Name);
        }
        public void FollowStatus(IObservable<Status> Statuses)
        {
            UnFollowStatus();
            _followSubscription = Statuses.Subscribe(async status =>
            {
                switch (status.Code)
                {
                    case StatusCode.Start: // Start match and start emitting scores
                        await StartMatch();
                        break;
                    case StatusCode.Stop: // Stop match and stop emitting scores
                        await StopMatch();
                        break;
                    case StatusCode.Ready: //Make sure Program is running
                        await StartProgram();
                        break;
                    case StatusCode.Unknown:
                        break;
                    case StatusCode.Running:
                        break;
                    case StatusCode.Stopped:
                        break;
                    case StatusCode.ScoreReceived:
                        break;
                    default:
                        break;
                }
            });
        }
        public void UnFollowStatus()
        {
            _followSubscription?.Dispose();
            _followSubscription = null;
        }
        public async Task<bool> Connect()
        {
            if (string.IsNullOrWhiteSpace(Connection))
                return false;
            using (await semaphore.LockAsync())
                if (_brick?.Connection.IsConnected ?? false)
                    return true;
            try
            {
                _brick = new Brick<I2CSensor, I2CSensor, I2CSensor, I2CSensor>(Connection);
                _brick.Connection.Open();
                using (await semaphore.LockAsync())
                {
                    try
                    {
                        _brick.StopProgram();
                    }
                    catch
                    { }
                    return _brick.Connection.IsConnected;
                }
            }
            catch
            {
                _brick?.Connection.Close();
                Connection = "";
                return false;
            }
        }

        private async Task StartMatch()
        {
            // If match is already running, don't start it again
            if (MatchIsRunning)
                return;
            // Start program, then wait for "Ready" before sending Start
            await StartProgram();
            _statuses.FirstAsync(s => s.Code == StatusCode.Ready)
                .Subscribe(async s =>
                {
                    if (Connected)
                    {
                        using (await semaphore.LockAsync())
                        {
                            _brick?.Mailbox?.Send("Start", STATUS_INBOX);
                        }
                        MatchStarted = true;
                    }
                });
        }
        private async Task StopMatch()
        {
            // If match/program is already stopped, don't stop it again
            if (!ProgramIsRunning)
                return;
            if (Connected)
                using (await semaphore.LockAsync())
                    _brick?.Mailbox?.Send("Stop", STATUS_INBOX);
            await StopProgram();
        }
        private async Task StartProgram()
        {
            if (ProgramIsRunning)
                return;
            if (Connected)
            {
                try
                {
                    using (await semaphore.LockAsync())
                        _brick?.StartProgram(Program);
                    ProgramStarted = true;
                    await StartReadMailboxes();
                }
                catch (MonoBrick.ConnectionException)
                {
                    Remove();
                }
            }
        }
        private void Remove() => _remover.Remove(Connection);
        private async Task StopProgram()
        {
            if (Connected)
            {
                try
                {
                    using (await semaphore.LockAsync())
                        _brick?.StopProgram();
                    ProgramStarted = false;
                    await StopReadMailboxes();
                    PostStatus(StatusCode.Stopped);
                }
                catch (MonoBrick.ConnectionException)
                {
                    Remove();
                }
            }
            MatchStarted = false;
        }
        private bool ProgramIsRunning
        {
            get
            {
                if (!ProgramStarted)
                    return false;
                using (semaphore.Lock())
                    try
                    {
                        return !string.IsNullOrWhiteSpace(_brick.GetRunningProgram().TrimEnd('\0'));
                    }
                    catch
                    {
                        return false;
                    }
            }
        }

        private bool MatchIsRunning => ProgramIsRunning && MatchStarted;

        private async void ReadMailboxes(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                while (!Connected)
                    await Task.Delay(500);
                if (!ProgramIsRunning)
                    continue;
                // Post all queued statuses
                using (await semaphore.LockAsync())
                    try
                    {
                        while (PostStatus(_brick.Mailbox.ReadString(STATUS_OUTBOX, true).TrimEnd('\0'))) ;
                    }
                    catch { }
                // Post all queued scores
                using (await semaphore.LockAsync())
                    try
                    {
                        while (_scoreFactory != null
                            && PostScore(_scoreFactory(_brick.Mailbox.ReadString(SCORE_OUTBOX, true).TrimEnd('\0')))) ;
                    }
                    catch { }
                await Task.Delay(100);
            }
        }
        private bool PostScore(Score score)
        {
            if (score == null)
                return false;
            _scores?.OnNext(score);
            return true;
        }
        private bool PostStatus(string status) => string.IsNullOrWhiteSpace(status)
                ? false
                : PostStatus(status.ToStatus());
        private Status Status(StatusCode code) =>
            new Status(code, typeof(Nxt), BrickName + Connection);
        private bool PostStatus(StatusCode code) =>
            PostStatus(Status(code));
        private bool PostStatus(Status status)
        {
            if (status.Code == StatusCode.Unknown)
                return false;
            _statuses?.OnNext(status);
            return true;
        }

        private async Task StartReadMailboxes()
        {
            await StopReadMailboxes();
            _messageTokenSource = new CancellationTokenSource();
            var token = _messageTokenSource.Token;
            _messageTask = Task.Factory.StartNew(() => ReadMailboxes(token), token);
        }

        private async Task StopReadMailboxes()
        {
            try
            {
                _messageTokenSource?.Cancel();
            }
            catch
            { }
            finally
            {
                await _messageTask;
                _messageTokenSource?.Dispose();
                await ClearOutboxes();
            }
        }

        private async Task ClearOutboxes()
        {
            using (await semaphore.LockAsync())
            {
                try
                {
                    while (_brick?.Mailbox?.ReadString(STATUS_OUTBOX, true) != null)
                    { }
                }
                catch
                { }
                try
                {
                    while (_brick?.Mailbox?.ReadString(SCORE_OUTBOX, true) != null)
                    { }
                }
                catch
                { }
            }
        }

        public async void Dispose()
        {
            await StopMatch();
            await StopReadMailboxes();
            UnFollowStatus();
            _brick?.Connection.Close();
            _scores?.OnCompleted();
            _scores?.Dispose();
            _statuses?.OnCompleted();
            _statuses?.Dispose();
            semaphore?.Dispose();
        }

        public bool Equals(Nxt other)
        {
            if (other == null) return false;
            return string.Equals(Connection, other.Connection);
        }
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Nxt);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return 17071 ^ Connection.GetHashCode();
            }
        }
    }
}