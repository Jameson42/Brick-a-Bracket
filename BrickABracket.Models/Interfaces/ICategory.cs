using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    public interface ICategory: IDBItem
    {
        string Name {get;}
        IClassification Classification {get;}
        IList<IRound> Rounds {get;}
        ITournament Tournament {get;}
        // TODO: Results
    }
}