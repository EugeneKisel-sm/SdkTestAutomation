using Xunit;

namespace SdkTestAutomation.Utils.Logging;

public class ConsoleLogger : ILogger
{
    private ITestContext TestContext { get; set; }
    public string Output { get; set; }
    public List<TestLog> AllLogs { get; set; }
    
    public ConsoleLogger(ITestContext context)
    {
        TestContext = context;
        AllLogs = new List<TestLog>();
    }
    
    public void Log(string message)
    {
        TestContext.TestOutputHelper.WriteLine(message);
        AllLogs.Add(new TestLog(message));
        Output += "\n" + message;
    }
    
    public override string ToString()
    {
        return Output;
    }
} 