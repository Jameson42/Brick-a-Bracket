using System;

namespace BrickABracket.Models.Interfaces
{
    public interface IMatchStarter 
    {
        IObservable<bool> Start {get;}
    }
}