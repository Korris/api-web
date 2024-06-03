namespace Mcsg.Api.Extensions
{
    public static class TimeExtension
    {
        public static string ToLabel(this DateTime dt, string format = "yy MMMM")
        {
            format = !string.IsNullOrEmpty(format) ? format : "yy MMMM";
            return dt.ToString(format);
        }
        public static int LastDay(this DateTime dt)
        {
            DateTime newDate = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            return newDate.Day;
        }
        public static DateTime FirstDate(this DateTime dt)
        {
            DateTime firstDate = new DateTime(dt.Year, dt.Month, 1);
            return firstDate;
        }
        public static DateTime LastDate(this DateTime dt)
        {
            var firstDate = FirstDate(dt);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);
            return lastDate;
        }
        public static DateTime BeginOfYear(this int year)
        {
            return new DateTime(year, 1, 1);
        }
        public static DateTime EndOfYear(this int year)
        {
            return (new DateTime(year + 1, 1, 1)).AddSeconds(-1);
        }
        public static DateTime BeginOfMonth(this int month)
        {
            var currentYear = DateTime.UtcNow.Year;
            return new DateTime(currentYear, month, 1);
        }
        public static DateTime EndOfMonth(this int month)
        {
            var currentYear = DateTime.UtcNow.Year;
            return (new DateTime(currentYear, month + 1, 1)).AddSeconds(-1);
        }
        public static DateTime BeginOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
    }
}
