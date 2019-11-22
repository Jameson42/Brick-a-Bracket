namespace BrickABracket.Models.Base
{
    public class Score
    {
        public Score(string s)
        {
            if (s == null || s.Length < 3)
                return;
            // NXT code starts with 1, not 0
            if (int.TryParse(s.Substring(0, 1), out int player))
                Player = player - 1;
            if (double.TryParse(s.Substring(2), out double time))
                Time = time;
        }
        public Score(int player, double time)
        {
            this.Player = player;
            this.Time = time;
        }
        public Score()
        {
            Player = 0;
            Time = 0.0;
        }
        public int Id { get; set; }
        public int Player { get; set; }
        public double Time { get; set; }

        public override string ToString() => $"{Player}-{Time}";

        public static implicit operator string(Score s) => s?.ToString() ?? string.Empty;
        public static implicit operator Score(string s)
        {
            return new Score(s);
        }
    }
}