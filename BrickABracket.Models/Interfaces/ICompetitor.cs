namespace BrickABracket.Models.Interfaces
{
    public interface ICompetitor: IDBItem
    {
        string FirstName {get;}
        string LastName {get;}
        string Name {get;}
        // Picure URI will be a REST endpoint for upload and download
        string PictureUri {get;}
    }
}