using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    public interface ITournament: IDBItem
    {
        IList<ICategory> Categories {get;}
        // TODO: Results
    }
}