using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Moc: IMoc
    {
        public delegate Moc Factory(string baseUrl);
        public Moc(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public string Name {get;set;}
        public string PictureUri => _baseUrl + _id;
        public IClassification Classification {get;set;}
        public ICompetitor Competitor {get;set;}
        public double Weight {get;set;}
        public int _id {get;set;}

        private string _baseUrl;
    }
}