namespace BrickABracket.Models.Interfaces
{
    public interface ICompetitor: IDBItem
    {
        string FirstName {get;set;}
        string LastName {get;set;}
        string Name {get;set;}
    }
}