using System;
using System.Reactive;
using System.Reactive.Subjects;

namespace BrickABracket.Models.Interfaces
{
    public interface IScoreProvider
    {
        IObservable<IScore> Scores {get;}
    }
}