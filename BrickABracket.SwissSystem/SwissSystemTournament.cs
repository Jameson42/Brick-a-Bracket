using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SwissSystem
{
    public class SwissSystemTournament : ITournament
    {
        public IList<ICategory> Categories {get;} = new List<ICategory>();
        public int _id { get; set; }
        public string Type => "swisssystem";

        // TODO: MOC list, create categories
    }
}
