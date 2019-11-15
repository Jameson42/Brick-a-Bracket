using System.Diagnostics.CodeAnalysis;
namespace BrickABracket.Models.Interfaces
{
    public interface IDBItem
    {
        // LiteDB prefers "_id" field name for item id
        [SuppressMessage("", "IDE1006:NamingRuleViolation")]
        int _id { get; set; }
    }
}