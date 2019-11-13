using System;
using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    public interface IDevice : IDisposable, IScoreProvider, IStatusFollower, IStatusProvider
    {
        bool Connect();
        bool Connected { get; }
        string Connection { get; }
        ushort BatteryLevel { get; }
        string BrickName { get; }
        string Program { get; set; }

        IEnumerable<string> GetPrograms();
    }
}