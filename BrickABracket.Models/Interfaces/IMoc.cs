namespace BrickABracket.Models.Interfaces
{
    public interface IMoc: IDBItem
    {
        string Name {get;set;}
        // Picure URI will be a REST endpoint for upload and download
        string PictureUri {get;}
        IClassification Classification {get;set;}
        ICompetitor Competitor {get;set;}
        double Weight {get;set;}
    }
}