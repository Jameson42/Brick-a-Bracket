using System;
using BrickABracket.Models.Base;

namespace BrickABracket.Models.Interfaces
{
    public interface IStatusProvider
    {
        IObservable<Status> Statuses { get; }
    }
}