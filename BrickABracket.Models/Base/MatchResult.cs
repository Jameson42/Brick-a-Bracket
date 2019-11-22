using System.Collections.Generic;

namespace BrickABracket.Models.Base
{
    public class MatchResult
    {
        public int Id { get; set; }
        public List<Score> Scores { get; set; } = new List<Score>();
    }
}