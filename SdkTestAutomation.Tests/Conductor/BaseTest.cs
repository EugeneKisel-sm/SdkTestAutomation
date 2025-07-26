using SdkTestAutomation.Api.Conductor.EventResource;
using SdkTestAutomation.Api.Conductor.WorkflowResource;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;
using RestSharp;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public abstract class BaseTest : IDisposable
{
    private readonly ILogger _logger;
    private readonly ResponseComparer _responseComparer;
    private AdapterFactory AdapterFactory { get; }

    #region Sdk

    protected IEventResourceAdapter EventResourceAdapter { get; }
    protected IWorkflowResourceAdapter WorkflowResourceAdapter { get; }

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
        
        _responseComparer = new ResponseComparer(_logger);
        AdapterFactory = new AdapterFactory(_logger, TestConfig.SdkType);
        EventResourceAdapter = AdapterFactory.CreateEventResourceAdapter();
        WorkflowResourceAdapter = AdapterFactory.CreateWorkflowResourceAdapter();
        
        EventResourceApi = new EventResourceApi(_logger);
        WorkflowResourceApi = new WorkflowResourceApi(_logger);
        
        _logger.Log($"Test '{testContext.TestCase?.TestCaseDisplayName}' execution started.");
        
    }
    
    protected bool ValidateSdkResponse<T>(SdkResponse<T> sdkResponse, RestResponse<T> apiResponse)
    {
        return _responseComparer.Compare(sdkResponse, apiResponse);
    }
    
    public virtual void Dispose()
    {
        EventResourceAdapter?.Dispose();
        WorkflowResourceAdapter?.Dispose();
        _logger.Log($"Test '{TestContext.Current.TestCase?.TestCaseDisplayName}' completed.");
    }
} 