using System;
using BrickABracket.Models.Base;

namespace BrickABracket.Models.Interfaces
{
    public interface IScoreProvider
    {
        IObservable<Score> Scores { get; }
    }
}