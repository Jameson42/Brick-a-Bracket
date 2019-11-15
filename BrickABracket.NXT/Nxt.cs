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
        private readonly object MessageLock = new object();
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
            _scoreFactory = scoreFactory;
            _scores = new Subject<Score>();
            _statuses = new BehaviorSubject<Status>(Status.Unknown);
            _remover = remover;
            Scores = _scores.AsObservable();
            Statuses = _statuses.AsObservable();

            Connection = connectionString;
        }

        public static string[] Ports => MonoBrick.Bluetooth<Command, Reply>
            .GetPortNames()
            .Append("USB")
            .ToArray();

        public bool Connected => Connect();
        public string Connection { get; private set; }
        public ushort BatteryLevel
        {
            get
            {
                lock (MessageLock)
                    try { return _brick?.GetBatteryLevel() ?? 0; }
                    catch { return 0; }
            }
        }
        public string BrickName
        {
            get
            {
                lock (MessageLock)
                    try { return _brick?.GetBrickName() ?? ""; }
                    catch { return ""; }
            }
        }
        public string Program { get; set; }

        public IEnumerable<string> GetPrograms()
        {
            lock (MessageLock)
                return _brick.FileSystem.FileList()
                    .Where(bf => bf.FileType == MonoBrick.FileType.Program)
                    .Select(bf => bf.Name);
        }
        public void FollowStatus(IObservable<Status> Statuses)
        {
            UnFollowStatus();
            _followSubscription = Statuses.Subscribe(status =>
            {
                switch (status)
                {
                    case Status.Start: // Start matach and start emitting scores
                        StartMatch();
                        break;
                    case Status.Stop: // Stop match and stop emitting scores
                        StopMatch();
                        break;
                    case Status.Ready: //Make sure Program is running
                        StartProgram();
                        break;
                    case Status.Unknown:
                        break;
                    case Status.Running:
                        break;
                    case Status.Stopped:
                        break;
                    case Status.ScoreReceived:
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
        public bool Connect()
        {
            lock (MessageLock)
                if (_brick?.Connection.IsConnected ?? false)
                    return true;
            if (string.IsNullOrWhiteSpace(Connection))
                return false;
            try
            {
                _brick = new Brick<I2CSensor, I2CSensor, I2CSensor, I2CSensor>(Connection);
                _brick.Connection.Open();
                lock (MessageLock)
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

        private void StartMatch()
        {
            // If match is already running, don't start it again
            if (MatchIsRunning)
                return;
            // Start program, then wait for "Ready" before sending Start
            StartProgram();
            _statuses.FirstAsync(s => s == Status.Ready)
                .Subscribe(s =>
                {
                    if (Connected)
                    {
                        lock (MessageLock)
                        {
                            _brick?.Mailbox?.Send("Start", STATUS_INBOX);
                            MatchStarted = true;
                        }
                    }
                });
        }
        private void StopMatch()
        {
            // If match/program is already stopped, don't stop it again
            if (!ProgramIsRunning)
                return;
            if (Connected)
                lock (MessageLock)
                    _brick?.Mailbox?.Send("Stop", STATUS_INBOX);
            StopProgram();
        }
        private void StartProgram()
        {
            if (ProgramIsRunning)
                return;
            if (Connected)
            {
                try
                {
                    lock (MessageLock)
                        _brick?.StartProgram(Program);
                    ProgramStarted = true;
                    StartReadMailboxes();
                }
                catch (MonoBrick.ConnectionException)
                {
                    Remove();
                }
            }
        }
        private void Remove() => _remover.Remove(Connection);
        private void StopProgram()
        {
            if (Connected)
            {
                try
                {
                    lock (MessageLock)
                        _brick?.StopProgram();
                    ProgramStarted = false;
                    StopReadMailboxes();
                    PostStatus(Status.Stopped);
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
                lock (MessageLock)
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

        private void ReadMailboxes(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                while (!Connected)
                    Thread.Sleep(500);
                if (!ProgramIsRunning)
                    continue;
                // Post all queued statuses
                lock (MessageLock)
                    try
                    {
                        while (PostStatus(_brick.Mailbox.ReadString(STATUS_OUTBOX, true).TrimEnd('\0'))) ;
                    }
                    catch { }
                // Post all queued scores
                lock (MessageLock)
                    try
                    {
                        while (_scoreFactory != null
                            && PostScore(_scoreFactory(_brick.Mailbox.ReadString(SCORE_OUTBOX, true).TrimEnd('\0')))) ;
                    }
                    catch { }
                Thread.Sleep(100);
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

        private bool PostStatus(Status status)
        {
            if (status == Status.Unknown)
                return false;
            _statuses?.OnNext(status);
            return true;
        }

        private void StartReadMailboxes()
        {
            StopReadMailboxes();
            _messageTokenSource = new CancellationTokenSource();
            var token = _messageTokenSource.Token;
            _messageTask = Task.Factory.StartNew(() => ReadMailboxes(token), token);
        }

        private void StopReadMailboxes()
        {
            try
            {
                _messageTokenSource?.Cancel();
            }
            catch
            { }
            finally
            {
                _messageTask?.Wait();
                _messageTokenSource?.Dispose();
                ClearOutboxes();
            }
        }

        private void ClearOutboxes()
        {
            lock (MessageLock)
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

        public void Dispose()
        {
            StopMatch();
            StopReadMailboxes();
            UnFollowStatus();
            _brick?.Connection.Close();
            _scores?.OnCompleted();
            _scores?.Dispose();
            _statuses?.OnCompleted();
            _statuses?.Dispose();
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