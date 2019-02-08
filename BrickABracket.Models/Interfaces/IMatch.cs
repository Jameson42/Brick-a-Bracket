using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    public interface IMatch: IDBItem
    {
        IList<IMoc> Mocs {get;}
        IRound Round {get;}
        // TODO: Results
    }
}