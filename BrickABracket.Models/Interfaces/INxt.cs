using System;

namespace BrickABracket.Models.Interfaces
{
    public interface INxt: IDisposable, IScoreProvider, IMatchFollower, IMatchStarter
    {
        bool Connect();
        bool Connect(string connection);
        bool Connected {get;}
        string Connection {get;}
        ushort BatteryLevel {get;}
        string BrickName {get;}

    }
}