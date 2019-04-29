using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    /// <summary>
    /// A specific grouping of MOCs within a round,
    /// i.e. a pair of MOCs for single elimination brackets, 
    /// or 4 MOCs for derby races
    /// </summary>
    public class Match
    {
        public List<int> MocIds {get;} = new List<int>();
        public List<Standing> Standings {get;set;} = new List<Standing>();
        // TODO: Need a good way to store multiple sets of times for the same match
    }
}