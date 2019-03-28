using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    /// <summary>
    /// Tournaments provide the logic to run a competition
    ///  Using DI-provided implementations of BrickABracket.Models.Interfaces
    ///  All logic is in the Tournament, not the sub-items.
    /// </summary>
    public interface ITournament: IDBItem
    {
        IList<ICategory> Categories {get;}
        string Type {get;}
        // TODO: Results, MOC list, ?
    }
}