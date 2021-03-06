using System.Collections.Generic;

namespace BrickABracket.Models.Base
{
    /// <summary>
    /// A set of rounds within a tournament, specific to a classification of MOCs
    /// </summary>
    public class Category
    {
        public Category(int classificationId)
        {
            ClassificationId = classificationId;
        }
        public Category() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public int ClassificationId { get; set; }
        public List<Round> Rounds { get; set; } = new List<Round>();
        public List<int> MocIds { get; set; } = new List<int>();
        public List<Standing> Standings { get; set; } = new List<Standing>();
    }
}