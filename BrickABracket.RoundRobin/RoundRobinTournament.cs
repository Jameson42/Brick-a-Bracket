using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.RoundRobin
{
    public class RoundRobinTournament: ITournament
    {
        public IList<ICategory> Categories {get;} = new List<ICategory>();
        public int _id { get; set; }
        public string Type => "roundrobin";

        // TODO: MOC list, create categories
    }
}
