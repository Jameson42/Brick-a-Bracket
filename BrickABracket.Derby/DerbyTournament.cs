using System;
using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Derby
{
    public class DerbyTournament : ITournament
    {
        public DerbyTournament()
        {
            Categories = new List<ICategory>();
        }
        public IList<ICategory> Categories {get;}
        public int _id { get; set; }

        // TODO: MOC list, create categories
    }
}
