using System.Globalization;

namespace SdkTestAutomation.Utils.Extensions;

public static class DateTimeExtensions
{
    private const string UtcDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffd";

    public static string ToUtcString(this DateTime dateTime)
    {
        return dateTime.ToString(UtcDateTimeFormat, CultureInfo.InvariantCulture);
    }
}