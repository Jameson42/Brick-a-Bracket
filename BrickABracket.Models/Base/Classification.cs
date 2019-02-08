using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Classification : IClassification
    {
        public Classification(string name)
        {
            Name = name;
        }
        public string Name {get;set;}
        public int _id { get; set; }
    }
}