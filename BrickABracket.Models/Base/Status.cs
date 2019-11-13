namespace BrickABracket.Models.Base
{
    public enum Status
    {
        Unknown,
        Ready,
        Running,
        Stopped,
        Start,
        Stop,
        ScoreReceived
    }
    public static class StatusExtensions
    {
        public static Status ToStatus(this string status)
        {
            return (status.ToLower()) switch
            {
                "ready" => Status.Ready,
                "start" => Status.Start,
                "stop" => Status.Stop,
                "running" => Status.Running,
                "stopped" => Status.Stopped,
                _ => Status.Unknown,
            };
        }
        public static Status ParseStatus(string status)
        {
            return status == null ? Status.Unknown : status.ToStatus();
        }
    }
}