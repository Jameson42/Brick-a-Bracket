using System;

namespace BrickABracket.Models.Base
{
    public class Status
    {
        public Status(StatusCode code, string senderName) : this(code, null, senderName)
        { }
        public Status(StatusCode code, Type senderType = null, string senderName = "")
        {
            Code = code;
            SenderType = senderType;
            SenderName = senderName == string.Empty
                ? senderType?.ToString() ?? string.Empty
                : senderName;
        }
        public StatusCode Code { get; }
        public Type SenderType { get; }
        public string SenderName { get; }
        public override string ToString() =>
            $"{SenderName}({SenderType} - {Code})";
    }
    public enum StatusCode
    {
        Unknown,
        Ready,
        Running,
        Stopped,
        Start,
        Stop,
        ScoreReceived
    }
    public static class StatusCodeExtensions
    {
        public static StatusCode ToStatus(this string status)
        {
            return (status.ToLower()) switch
            {
                "ready" => StatusCode.Ready,
                "start" => StatusCode.Start,
                "stop" => StatusCode.Stop,
                "running" => StatusCode.Running,
                "stopped" => StatusCode.Stopped,
                _ => StatusCode.Unknown,
            };
        }
        public static StatusCode ParseStatus(string status)
        {
            return status == null ? StatusCode.Unknown : status.ToStatus();
        }
    }
}