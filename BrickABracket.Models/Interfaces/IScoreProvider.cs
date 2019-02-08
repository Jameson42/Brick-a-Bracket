using System;

namespace BrickABracket.Models.Interfaces
{
    public interface IScoreProvider
    {
        IObservable<IScore> Scores {get;}
    }
}