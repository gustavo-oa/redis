using System;

namespace Redis.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TimeToExpireAttribute : FlagsAttribute
    {
        public int SecondsToExpire { get; set; } = 60;
    }
}
