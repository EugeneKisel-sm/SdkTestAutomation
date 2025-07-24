namespace SdkTestAutomation.Utils.Logging;

public interface ILogger
{
    string Output { get; set; }

    List<TestLog> AllLogs { get; set; }

    void Log(string message);
}