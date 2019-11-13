namespace BrickABracket.Models.Interfaces
{
    public interface ITournamentSummary : IDBItem
    {
        string TournamentType { get; set; }
        string Name { get; set; }
    }
}