using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Classification : IDBItem
    {
        public string Name { get; set; }
        public int _id { get; set; }
    }
}