using System;
using System.Diagnostics;

namespace BuildScreen.Core.Extensions
{
    [DebuggerStepThrough]
    public static class DoubleExtensions
    {
        public static DateTime ToDateTimeFromUnixTimestamp(this double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds(timestamp);
        }
    }
}
