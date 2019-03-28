using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Derby
{
    public class DerbyTournament : ITournament
    {
        public IList<ICategory> Categories {get;} = new List<ICategory>();
        public int _id { get; set; }
        public string Type => "derby";

        // TODO: MOC list, create categories
    }
}
