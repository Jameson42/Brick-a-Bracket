using System;
using BrickABracket.Models.Base;

namespace BrickABracket.Models.Interfaces
{
    public interface IMatchStarter 
    {
        IObservable<Status> Statuses {get;}
    }
}