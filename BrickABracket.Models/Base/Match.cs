using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Match : IMatch
    {
        public IList<IMoc> Mocs {get;} = new List<IMoc>();
        public IRound Round {get;set;}
        public int _id { get; set; }
    }
}