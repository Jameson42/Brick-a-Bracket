using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MonoBrick.NXT;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.NXT
{
    public class Nxt : IDevice, IEquatable<Nxt>
    {
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
        private bool Running { get; set; } = false;
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

        public static string[] BluetoothPorts
        {
            get
            {
                return MonoBrick.Bluetooth<Command, Reply>.GetPortNames();
            }
        }

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
            _followSubscription = Statuses.Subscribe(i =>
            {
                switch (i)
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
            if (_followSubscription != null)
            {
                _followSubscription.Dispose();
                _followSubscription = null;
            }
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
                StartReadMailboxes();
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
                StopReadMailboxes();
                return false;
            }
        }

        private void StartMatch()
        {
            //Start program, then wait for "Ready" before sending Start?
            StartProgram();
            _statuses.FirstAsync(s => s == Status.Ready)
                .Subscribe(s =>
                {
                    if (Connected)
                        lock (MessageLock)
                            _brick?.Mailbox?.Send("Start", Box.Box5);
                });
        }
        private void StopMatch()
        {
            if (Connected)
                lock (MessageLock)
                    _brick?.Mailbox?.Send("Stop", Box.Box5);
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
                    Running = true;
                }
                catch (MonoBrick.ConnectionException)
                {
                    Remove();
                }
            }
        }
        private void Remove()
        {
            _remover.Remove(Connection);
        }
        private void StopProgram()
        {
            if (Connected)
            {
                try
                {
                    lock (MessageLock)
                        _brick?.StopProgram();
                    Running = false;
                }
                catch (MonoBrick.ConnectionException)
                {
                    Remove();
                }
            }
        }
        private bool ProgramIsRunning
        {
            get
            {
                if (!Running)
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
                        while (PostStatus(_brick.Mailbox.ReadString(Box.Box0, true).TrimEnd('\0'))) ;
                    }
                    catch { }
                // Post all queued scores
                lock (MessageLock)
                    try
                    {
                        while (_scoreFactory != null
                            && PostScore(_scoreFactory(_brick.Mailbox.ReadString(Box.Box1, true).TrimEnd('\0')))) ;
                    }
                    catch { }
                Thread.Sleep(100);
            }
        }
        private bool PostScore(Score score)
        {
            if (score == null)
                return false;
            _scores.OnNext(score);
            return true;
        }
        private bool PostStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;
            return PostStatus(status.ToStatus());
        }

        private bool PostStatus(Status status)
        {
            if (status == Status.Unknown)
                return false;
            _statuses.OnNext(status);
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