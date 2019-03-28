using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Category : ICategory
    {
        public string Name {get;set;}
        public IClassification Classification {get;set;}
        public IList<IRound> Rounds {get;set;}
        public ITournament Tournament {get;set;}
        public int _id { get; set; }
    }
}