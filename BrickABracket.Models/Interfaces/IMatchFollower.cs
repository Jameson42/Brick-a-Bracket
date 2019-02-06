using System;
using System.Reactive;
using BrickABracket.Models.Base;

namespace BrickABracket.Models.Interfaces
{
    public interface IMatchFollower
    {
        void FollowStart(IObservable<Status> Start);
    }
}