namespace GreenRoam.Ultilities
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo VietnamTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public static DateTime ConvertToVietnamTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Local)
                utcDateTime = utcDateTime.ToUniversalTime(); // đảm bảo là UTC

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
        }

        public static DateTime NowVietnamTime()
        {
            return ConvertToVietnamTime(DateTime.UtcNow);
        }
    }
}
