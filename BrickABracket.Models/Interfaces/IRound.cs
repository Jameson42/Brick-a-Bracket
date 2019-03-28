using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    public interface IRound: IDBItem
    {
        IList<IMatch> Matches {get;}
        ICategory Category {get;set;}
    }
}