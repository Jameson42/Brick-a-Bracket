using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Tournament : TournamentSummary, IDBItem
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<int> MocIds { get; set; } = new List<int>();
        // Inherited properties:
        // public string TournamentType { get; set; }
        // public string Name { get; set; }
        // public int _id { get; set; }
    }
}
