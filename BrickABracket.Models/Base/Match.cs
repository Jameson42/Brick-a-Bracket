using System.Collections.Generic;

namespace BrickABracket.Models.Base
{
    /// <summary>
    /// A specific grouping of MOCs within a round,
    /// i.e. a pair of MOCs for single elimination brackets, 
    /// or 4 MOCs for derby races
    /// </summary>
    public class Match
    {
        public int Id { get; set; }
        public List<int> MocIds { get; set; } = new List<int>();
        public List<MatchResult> Results { get; set; } = new List<MatchResult>();
    }
}