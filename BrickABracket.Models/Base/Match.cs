using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Match : IDBItem
    {
        public List<int> MocIds {get;} = new List<int>();
        public int _id { get; set; }
    }
}