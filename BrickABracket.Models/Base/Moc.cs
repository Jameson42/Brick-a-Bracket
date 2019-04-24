using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    /// <summary>
    /// A built competitive creation, submitted by a competitor
    /// </summary>
    public class Moc: IDBItem
    {
        public string Name {get;set;}
        public int ClassificationId {get;set;}
        public int CompetitorId {get;set;}
        public double Weight {get;set;}
        public int _id {get;set;}
    }
}