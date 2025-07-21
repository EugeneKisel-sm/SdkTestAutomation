using System.Collections.Concurrent;
using SdkTestAutomation.Api.Conductor.EventResource;
using SdkTestAutomation.Api.Conductor.WorkflowResource;
using SdkTestAutomation.Utils.Logging;
using Xunit;
using Xunit.Sdk;

namespace SdkTestAutomation.Tests.Conductor;

public abstract class BaseTest : IDisposable
{
    private readonly ILogger _logger;

    private ITestCase TestCase { get;}
    private static ConcurrentDictionary<string, int> Iterations { get; set; } = new ConcurrentDictionary<string, int>();
    protected EventResourceApi EventResourceApi { get; }
    protected WorkflowResourceApi WorkflowResourceApi { get; }
    
    protected BaseTest()
    {
        var testContext = TestContext.Current;
        TestCase = TestContext.Current.TestCase;
        _logger = new ConsoleLogger(testContext);
        if (Iterations.ContainsKey(TestCase.TestCaseDisplayName))
        {
            Iterations[TestCase.TestCaseDisplayName]++;
        }
        else
        {
            Iterations.TryAdd(TestCase.TestCaseDisplayName, 1);
        }

        _logger.Log($"Test '{TestCase.TestCaseDisplayName}' execution started.");
        
        EventResourceApi = new EventResourceApi(_logger);
        WorkflowResourceApi = new WorkflowResourceApi(_logger);
    }

    public virtual void Dispose()
    {
        _logger.Log($"Test '{TestCase.TestCaseDisplayName}' completed with {TestContext.Current.TestStatus}.");
    }
} 