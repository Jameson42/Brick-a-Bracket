using System.Collections.Generic;

namespace BrickABracket.Models.Interfaces
{
    public interface ICategory {
        string Name {get;}
        List<IRound> Rounds {get;}
    }
}