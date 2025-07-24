using Xunit;

namespace SdkTestAutomation.Utils.Logging;

public class ConsoleLogger(ITestContext context) : ILogger
{
    private ITestContext TestContext { get; set; } = context;
    public string Output { get; set; }
    public List<TestLog> AllLogs { get; set; } = new();

    public void Log(string message)
    {
        if (TestContext?.TestOutputHelper != null)
        {
            TestContext.TestOutputHelper.WriteLine(message);
        }
        else
        {
            Console.WriteLine(message);
        }

        AllLogs.Add(new TestLog(message));
        Output += "\n" + message;
    }

    public override string ToString()
    {
        return Output;
    }
}