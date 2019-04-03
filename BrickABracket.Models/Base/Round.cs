using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Round: IDBItem
    {
        public List<Match> Matches {get;} = new List<Match>();
        public int _id {get;set;}
    }
}