using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Tournament : IDBItem
    {
        public List<Category> Categories {get;} = new List<Category>();
        public int _id { get; set; }
        public string TournamentType {get;set;}

        // TODO: MOC list, create categories
    }
}
