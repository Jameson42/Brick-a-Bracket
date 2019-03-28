using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Round: IRound
    {
        public IList<IMatch> Matches {get;} = new List<IMatch>();
        public ICategory Category {get;set;}
        public int _id {get;set;}
    }
}