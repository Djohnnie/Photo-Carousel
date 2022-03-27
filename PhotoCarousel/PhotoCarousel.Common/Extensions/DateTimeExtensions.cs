using System;

namespace PhotoCarousel.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundToMinutes(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, 0, value.Kind);
        }
    }
}