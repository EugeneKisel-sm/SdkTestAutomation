using SdkTestAutomation.Sdk.Core;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Implementations.Go;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;
using SdkTestAutomation.Api.Orkes.TokenResource;
using Xunit;

namespace SdkTestAutomation.Tests.Orkes;

public abstract class BaseOrkesTest : IDisposable
{
    private readonly ILogger _logger;

    #region Sdk

    protected IEventAdapter EventAdapter { get; }
    protected IWorkflowAdapter WorkflowAdapter { get; }

    #endregion

    #region OrkesApi

    protected TokenResourceApi TokenResourceApi { get; }

    #endregion
    
    protected BaseOrkesTest()
    {
        var testContext = TestContext.Current;
        _logger = new ConsoleLogger(testContext);
        _logger.Log($"Using SDK type: {TestConfig.SdkType}");
        
        EventAdapter = SdkFactory.CreateEventAdapter(TestConfig.SdkType);
        WorkflowAdapter = SdkFactory.CreateWorkflowAdapter(TestConfig.SdkType);
        
        var serverUrl = TestConfig.ApiUrl;
        EventAdapter.Initialize(serverUrl);
        WorkflowAdapter.Initialize(serverUrl);
        
        TokenResourceApi = new TokenResourceApi(_logger);
        
        _logger.Log($"Test '{testContext.TestCase?.TestCaseDisplayName}' execution started.");
    }
    
    public virtual void Dispose()
    {
        if (TestConfig.SdkType == "go")
        {
            LogGoSdkDetails();
        }
        
        EventAdapter?.Dispose();
        WorkflowAdapter?.Dispose();
        _logger.Log($"Test '{TestContext.Current.TestCase?.TestCaseDisplayName}' completed.");
    }
    
    private void LogGoSdkDetails()
    {
        try
        {
            if (EventAdapter is GoEventAdapter goEventAdapter)
            {
                var eventLogs = goEventAdapter.GetLogs();
                if (!string.IsNullOrEmpty(eventLogs))
                {
                    _logger.Log("=== GO SDK EVENT ADAPTER LOGS ===");
                    _logger.Log(eventLogs);
                    _logger.Log("=== END GO SDK EVENT ADAPTER LOGS ===");
                    goEventAdapter.ClearLogs();
                }
            }
            
            if (WorkflowAdapter is GoWorkflowAdapter goWorkflowAdapter)
            {
                var workflowLogs = goWorkflowAdapter.GetLogs();
                if (!string.IsNullOrEmpty(workflowLogs))
                {
                    _logger.Log("=== GO SDK WORKFLOW ADAPTER LOGS ===");
                    _logger.Log(workflowLogs);
                    _logger.Log("=== END GO SDK WORKFLOW ADAPTER LOGS ===");
                    goWorkflowAdapter.ClearLogs();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Error retrieving Go SDK logs: {ex.Message}");
        }
    }
} 