using System;

namespace BrickABracket.Models.Base
{
    [Flags]
    public enum DeviceRole
    {
        None = 0x0,
        StatusProvider = 0x1,
        StatusFollower = 0x2,
        ScoreProvider = 0x4,
        StartButton = 0x3,
        Timer = 0x6,
        All = 0x7
    }
}