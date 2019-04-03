using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Moc: IDBItem
    {
        public string Name {get;set;}
        public Classification Classification {get;set;}
        public Competitor Competitor {get;set;}
        public double Weight {get;set;}
        public int _id {get;set;}
    }
}