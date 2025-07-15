using System.Globalization;

namespace SdkTestAutomation.Utils.Logging;

public class TestLog
{
    public TestLog(string message)
    {
        Message = message;
        LogTime = DateTime.Now;
    }

    public TestLog() { }

    public string Message { get; set; }
    public DateTime LogTime { get; set; }

    public string LogTimeFormatted => LogTime.ToString(CultureInfo.InvariantCulture);
}