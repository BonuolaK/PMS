using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PMS.Shared.Helpers
{
    public static class Helper
    {
        public static DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }

        public static DateTime ToInvariantDateTime(this String value, String format, out bool succeeded)
        {
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.InvariantInfo;
            var result = DateTime.TryParseExact(value, format, dtfi, DateTimeStyles.None, out DateTime newValue);
            succeeded = result;
            return newValue;
        }

    }
}
