using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Category : IDBItem
    {
        public string Name {get;set;}
        public int ClassificationId {get;set;}
        public List<Round> Rounds {get;set;} = new List<Round>();
        public int _id { get; set; }
    }
}