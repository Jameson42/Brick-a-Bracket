using System.Collections.Generic;

namespace BrickABracket.Models.Base
{
    public class TournamentPattern
    {
        public string _id {get;set;}
        public List<RoundPattern> Rounds {get;} = new List<RoundPattern>();
    }
}