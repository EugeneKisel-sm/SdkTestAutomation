using SdkTestAutomation.Api.Conductor.EventResource;
using SdkTestAutomation.Api.Conductor.WorkflowResource;
using SdkTestAutomation.Sdk.Core;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;
using RestSharp;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public abstract class BaseTest : IDisposable
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
    
    protected BaseTest()
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
        EventAdapter?.Dispose();
        WorkflowAdapter?.Dispose();
        _logger.Log($"Test '{TestContext.Current.TestCase?.TestCaseDisplayName}' completed.");
    }
} 