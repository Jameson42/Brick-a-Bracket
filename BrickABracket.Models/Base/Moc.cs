using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Moc: IDBItem
    {
        public delegate Moc Factory(string baseUrl);
        public Moc(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public string Name {get;set;}
        public string PictureUri => _baseUrl + _id;
        public Classification Classification {get;set;}
        public Competitor Competitor {get;set;}
        public double Weight {get;set;}
        public int _id {get;set;}

        private string _baseUrl;
    }
}