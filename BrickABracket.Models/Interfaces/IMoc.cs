namespace BrickABracket.Models.Interfaces
{
    public interface IMoc
    {
        string Name {get;}
        // Picure URI will be a REST endpoint for upload and download
        string PictureUri {get;}
    }
}