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
        public List<Match> Matches {get;set;} = new List<Match>();
        public List<Standing> Standings {get;set;} = new List<Standing>();
    }
}