using System.Collections.Generic;

namespace BrickABracket.Models.Base
{
    public class RoundPattern
    {
        public List<MatchPattern> Matches {get;} = new List<MatchPattern>();
    }
}