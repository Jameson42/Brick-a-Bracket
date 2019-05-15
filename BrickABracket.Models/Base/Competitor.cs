using System.Linq;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Competitor : IDBItem
    {
        public string Name {get;set;}
        public int _id {get;set;}
    }
}