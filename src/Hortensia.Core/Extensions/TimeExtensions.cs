using System;
using System.Text;

namespace Hortensia.Core.Extensions
{
    public static class TimeExtensions
    {
        public static double DateTimeToUnixTimestamp(this DateTime dateTime)
            => (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;


        public static int DateTimeToUnixTimestampSeconds(this DateTime dateTime)
            => (int)(dateTime - new DateTime(1970, 1, 1, 1, 0, 0).ToLocalTime()).TotalSeconds;
        public static string ToFileNameDate(this DateTime dateTime)
            => string.Format("{0}-{1}-{2}_{3}h{4}m{5}s", dateTime.Day, dateTime.Month, dateTime.Year, dateTime.Hour, dateTime.Minute, dateTime.Second);
        public static DateTime UnixTimestampToDateTime(this double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimestampToDateTime(this int unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static double ConvertToMinutes(this double milliseconds)
            => milliseconds * 0.0000166667;

        public static string ToPrettyFormat(this DateTime span)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (span.Second != 0)
            {
                if (span.Hour > 0)
                    stringBuilder.AppendFormat("{0} hour{1} ", span.Hour, (span.Hour > 1 ? "h" : string.Empty));

                if (span.Minute > 0)
                    stringBuilder.AppendFormat("{0} minute{1} ", span.Minute, (span.Minute > 1 ? "m" : string.Empty));

                if (span.Second > 0)
                    stringBuilder.AppendFormat("{0} second{1} ", span.Second, (span.Second > 1 ? "s" : string.Empty));
            }

            else return "0 seconds ";

            return stringBuilder.ToString();
        }
    }
}
