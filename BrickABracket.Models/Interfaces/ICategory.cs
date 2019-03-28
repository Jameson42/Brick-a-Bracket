using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    public interface ICategory: IDBItem
    {
        string Name {get;set;}
        IClassification Classification {get;set;}
        IList<IRound> Rounds {get;set;}
        ITournament Tournament {get;set;}
    }
}