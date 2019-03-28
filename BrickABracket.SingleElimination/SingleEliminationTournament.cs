using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SingleElimination
{
    public class SingleEliminationTournament : ITournament
    {
        public IList<ICategory> Categories {get;} = new List<ICategory>();
        public int _id { get; set; }
        public string Type => "singleelimination";

        // TODO: MOC list, create categories
    }
}
