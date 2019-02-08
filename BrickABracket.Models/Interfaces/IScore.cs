namespace BrickABracket.Models.Interfaces
{
    public interface IScore: IDBItem
    {
        int player {get;}
        double time {get;}
    }
}