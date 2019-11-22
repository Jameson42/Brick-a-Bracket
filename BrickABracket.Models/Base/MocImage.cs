using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class MocImage : IDBItem
    {
        public int _id { get; set; }
        public byte[] File { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}