using System;
using BrickABracket.Models.Base;

namespace BrickABracket.Models.Interfaces
{
    public interface IStatusFollower
    {
        void FollowStatus(IObservable<Status> Statuses);
    }
}