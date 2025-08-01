using SdkTestAutomation.Api.Conductor.EventResource;
using SdkTestAutomation.Api.Conductor.WorkflowResource;
using SdkTestAutomation.Sdk.Core;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;
using SdkTestAutomation.Sdk.Implementations.Go;
using SdkTestAutomation.Sdk.Implementations.Java.Conductor;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;
using RestSharp;
using Xunit;
using SdkTestAutomation.Sdk.Implementations.Go.Adapters;

namespace SdkTestAutomation.Tests.Conductor;

public abstract class BaseConductorTest : IDisposable
{
    private readonly ILogger _logger;

    #region Sdk

    protected IEventAdapter EventAdapter { get; }
    protected IWorkflowAdapter WorkflowAdapter { get; }

    #endregion

    #region ConductorApi

    protected EventResourceApi EventResourceApi { get; }
    protected WorkflowResourceApi WorkflowResourceApi { get; }

    #endregion
    
    protected BaseConductorTest()
    {
        var testContext = TestContext.Current;
        _logger = new ConsoleLogger(testContext);
        _logger.Log($"Using SDK type: {TestConfig.SdkType}");
        
        // Initialize SDK adapters
        EventAdapter = SdkFactory.CreateEventAdapter(TestConfig.SdkType);
        WorkflowAdapter = SdkFactory.CreateWorkflowAdapter(TestConfig.SdkType);
        
        // Initialize SDK adapters with server URL
        var serverUrl = TestConfig.ApiUrl;
        EventAdapter.Initialize(serverUrl);
        WorkflowAdapter.Initialize(serverUrl);
        
        EventResourceApi = new EventResourceApi(_logger);
        WorkflowResourceApi = new WorkflowResourceApi(_logger);
        
        _logger.Log($"Test '{testContext.TestCase?.TestCaseDisplayName}' execution started.");
    }
    
    protected bool ValidateSdkResponse(SdkResponse sdkResponse, RestResponse apiResponse)
    {
        // Simple validation - check if SDK call was successful
        return sdkResponse.Success && apiResponse.IsSuccessful;
    }
    
    public virtual void Dispose()
    {
        // Log SDK details based on type
        switch (TestConfig.SdkType)
        {
            case "go":
                LogGoSdkDetails();
                break;
            case "java":
                LogJavaSdkDetails();
                break;
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
    
    private void LogJavaSdkDetails()
    {
        try
        {
            _logger.Log("=== JAVA SDK DETAILS ===");

            if (EventAdapter is JavaEventAdapter javaEventAdapter)
            {
                var eventLogs = javaEventAdapter.GetAllJavaLogs();
                if (!string.IsNullOrEmpty(eventLogs))
                {
                    _logger.Log("=== JAVA CLI EVENT LOGS ===");
                    _logger.Log(eventLogs);
                    _logger.Log("=== END JAVA CLI EVENT LOGS ===");
                }
            }
            
            if (WorkflowAdapter is JavaWorkflowAdapter javaWorkflowAdapter)
            {
                var workflowLogs = javaWorkflowAdapter.GetAllJavaLogs();
                if (!string.IsNullOrEmpty(workflowLogs))
                {
                    _logger.Log("=== JAVA CLI WORKFLOW LOGS ===");
                    _logger.Log(workflowLogs);
                    _logger.Log("=== END JAVA CLI WORKFLOW LOGS ===");
                }
            }
            
            _logger.Log("=== END JAVA SDK DETAILS ===");
        }
        catch (Exception ex)
        {
            _logger.Log($"Error retrieving Java SDK details: {ex.Message}");
        }
    }
} 