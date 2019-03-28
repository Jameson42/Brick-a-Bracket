using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Score : IScore
    {
        public Score(string s)
        {
            if (s == null || s.Length<3)
                return;
            if (int.TryParse(s.Substring(0,1), out int player))
                this.player = player;
            if (double.TryParse(s.Substring(2), out double time))
                this.time = time;
        }
        public  Score(int player, double time)
        {
            this.player = player;
            this.time = time;
        }
        public int player {get;}
        public double time {get;}
        public int _id { get; set; }

        public override string ToString() => $"{player}-{time}";

        public static implicit operator string(Score s) => s?.ToString() ?? string.Empty;
        public static implicit operator Score(string s)
        {
            if (s == null || s.Length<3)
                return null;
            if (int.TryParse(s.Substring(0,1), out int player)
            && double.TryParse(s.Substring(2), out double time))
                return new Score(player, time);
            return null;
        }
    }
}