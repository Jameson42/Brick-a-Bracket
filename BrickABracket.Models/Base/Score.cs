using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Score : IScore
    {
        public  Score(int lane, double time)
        {
            this.lane = lane;
            this.time = time;
        }
        public int lane {get;}
        public double time {get;}
    }
}