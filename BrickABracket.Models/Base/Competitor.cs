using System.Linq;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Competitor : IDBItem
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name
        {
            get => FirstName + " " + LastName;
            set
            {
                var values = value?.Split(' ').ToList();
                FirstName = values?[0] ?? "";
                values?.RemoveAt(0);
                LastName = values?.Aggregate((t,a) => t + " " + a)?.Trim(' ') ?? "";
            }
        }
        public int _id {get;set;}
    }
}