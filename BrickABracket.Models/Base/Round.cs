using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    /// <summary>
    /// A group of Matches, i.e. each level of a single elimination bracket
    /// or a set of derby rounds
    /// </summary>
    public class Round
    {
        public List<Match> Matches {get;} = new List<Match>();
    }
}