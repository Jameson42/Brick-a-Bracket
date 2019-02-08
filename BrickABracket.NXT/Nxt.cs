using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;
using MonoBrick.NXT;

namespace BrickABracket.NXT
{
    public class Nxt : INxt
    {
        private Brick<I2CSensor,I2CSensor,I2CSensor,I2CSensor> _brick;
        private static readonly object MessageLock = new object();
        private IDisposable _followSubscription;
        private Subject<IScore> _scores;
        private Subject<Status> _statuses;
        private Thread _messageThread;

        public Nxt()
        {
            _scores = new Subject<IScore>();
            // call _scores.OnNext() with each new IScore
            _statuses = new Subject<Status>();
            // call _start.OnNext() with true with each NXT-triggered start. 
            // Core will aggregate with false on each stop.
            Scores = _scores.AsObservable();
            Statuses = _statuses.Replay(1); //Always provide current status to new subscribers

            _messageThread = new Thread(new ThreadStart(ReadMailboxes));
            _messageThread.IsBackground = true;
            _messageThread.Start();
        }

        public bool Connected => Connect();
        public string Connection { get; private set; }
        public ushort BatteryLevel {
            get
            {
                try {return _brick?.GetBatteryLevel() ?? 0;}
                catch {return 0;}
            }
        }
        public string BrickName {
            get
            {
                try {return _brick?.GetBrickName() ?? "";}
                catch {return "";}
            }
        }
        public void FollowStart(IObservable<Status> Statuses)
        {
            if (_followSubscription != null)
            {
                _followSubscription.Dispose();
                _followSubscription = null;
            }
            _followSubscription = Statuses.Subscribe(i => {
                if (i == Status.Start)  // Start race and start emitting scores
                    StartMatch();
                else if (i == Status.Stop) // Stop race and stop emitting scores
                    StopMatch();
            });
        }
        public IObservable<IScore> Scores { get; private set; }
        public IObservable<Status> Statuses { get; private set; }
        public bool Connect()
        {
            return Connect(null);
        }
        public bool Connect(string connection)
        {
            if (_brick?.Connection.IsConnected ?? false)
                return true;
            if (connection == null)
                return false;
            try
            {
                _brick = new Brick<I2CSensor, I2CSensor, I2CSensor, I2CSensor>(connection);
                _brick.Connection.Open();
                //_brick.Beep(50);
                Connection = connection;
                return _brick.Connection.IsConnected;
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
            lock(MessageLock)
            {
                _brick.Mailbox.Send("Start", Box.Box5);
            }
        }
        private void StopMatch()
        {
            lock(MessageLock)
            {
                _brick.Mailbox.Send("Stop", Box.Box5);
            }
        }

        private void ReadMailboxes()
        {
            while(true)
            {
                lock(MessageLock)
                {
                    // Post all queued statuses
                    while (PostStatus(_brick.Mailbox.ReadString(Box.Box0, true)));
                    // Post all queued scores
                    while (PostScore((Score)_brick.Mailbox.ReadString(Box.Box1, true)));
                }
                Thread.Sleep(500);
            }
        }
        private bool PostScore(IScore score)
        {
            if (score == null)
                return false;
            _scores.OnNext(score);
            return true;
        }
        private bool PostStatus(string status)
        {
            if (status == null)
                return false;
            return PostStatus(status.ToStatus());
        }

        private bool PostStatus(Status status)
        {
            if (status == BrickABracket.Models.Base.Status.Unknown)
                return false;
            _statuses.OnNext(status);
            return true;
        }

        public void Dispose()
        {
            StopMatch();
            _messageThread.Abort();
            _brick?.Connection.Close();
            _scores.OnCompleted();
            _scores.Dispose();
            _statuses.OnCompleted();
            _statuses.Dispose();
            _followSubscription.Dispose();
        }
    }
}