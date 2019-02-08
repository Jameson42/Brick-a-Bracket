namespace BrickABracket.Models.Base
{
    public enum Status
    {
        Unknown,
        Ready,
        Running,
        Stopped,
        Start,
        Stop        
    }
    public static class StatusExtensions
    {
        public static Status ToStatus(this string status)
        {
            switch (status.ToLower())
            {
                case "ready":
                    return Status.Ready;
                case "start":
                    return Status.Start;
                case "stop":
                    return Status.Stop;
                case "running":
                    return Status.Running;
                case "stopped":
                    return Status.Stopped;
                default:
                    return Status.Unknown;
            }
        }
        public static Status ParseStatus(string status)
        {
            if (status == null)
                return Status.Unknown;
            return status.ToStatus();
        }
    }
}