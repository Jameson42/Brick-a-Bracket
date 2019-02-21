using System;

namespace BrickABracket.Models.Interfaces
{
    public interface IDevice: IDisposable, IScoreProvider, IStatusFollower, IStatusProvider
    {
        bool Connect();
        bool Connect(string connection);
        bool Connected {get;}
        string Connection {get;}
        ushort BatteryLevel {get;}
        string BrickName {get;}

    }
}