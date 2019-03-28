using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Classification : IClassification
    {
        public string Name {get;set;}
        public int _id { get; set; }
    }
}