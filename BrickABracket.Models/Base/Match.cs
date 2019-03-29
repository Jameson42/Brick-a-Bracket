using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Match : IDBItem
    {
        public List<Moc> Mocs {get;} = new List<Moc>();
        public Round Round {get;set;}
        public int _id { get; set; }
    }
}