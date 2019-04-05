using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class TournamentSummary : IDBItem, ITournamentSummary
    {
        public string TournamentType {get;set;}
        public string Name {get;set;}
        public int _id { get; set; }
    }
}
