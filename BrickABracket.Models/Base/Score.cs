using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Score
    {
        public Score(string s)
        {
            if (s == null || s.Length<3)
                return;
            if (int.TryParse(s.Substring(0,1), out int player))
                Player = player;
            if (double.TryParse(s.Substring(2), out double time))
                Time = time;
        }
        public  Score(int player, double time)
        {
            this.Player = player;
            this.Time = time;
        }
        public Score()
        {
            Player = 0;
            Time = 0.0;
        }
        public int Player {get;}
        public double Time {get;}

        public override string ToString() => $"{Player}-{Time}";

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