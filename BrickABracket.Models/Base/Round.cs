using System.Collections.Generic;

namespace BrickABracket.Models.Base
{
    /// <summary>
    /// A group of Matches, i.e. each level of a single elimination bracket
    /// or a set of derby rounds
    /// </summary>
    public class Round
    {
        public int Id { get; set; }
        public List<Match> Matches { get; set; } = new List<Match>();
        public List<Standing> Standings { get; set; } = new List<Standing>();
        public List<int> MocIds { get; set; } = new List<int>();
    }
}