using SdkTestAutomation.Api.Conductor.EventResource;
using SdkTestAutomation.Api.Conductor.WorkflowResource;
using SdkTestAutomation.Sdk.Helpers;
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
    private IEventResourceAdapter _eventResourceAdapter;
    private IWorkflowResourceAdapter _workflowResourceAdapter;
    
    protected EventResourceApi EventResourceApi { get; }
    protected WorkflowResourceApi WorkflowResourceApi { get; }
    
    protected BaseTest()
    {
        var testContext = TestContext.Current;
        _logger = new ConsoleLogger(testContext);
        _responseComparer = new ResponseComparer(_logger);
        
        EventResourceApi = new EventResourceApi(_logger);
        WorkflowResourceApi = new WorkflowResourceApi(_logger);
        
        _logger.Log($"Test '{testContext.TestCase?.TestCaseDisplayName}' execution started.");
        _logger.Log($"Using SDK type: {TestConfig.SdkType}");
    }
    
    protected async Task<IEventResourceAdapter> GetEventResourceAdapterAsync()
    {
        _eventResourceAdapter ??= await AdapterFactory.CreateEventResourceAdapterAsync(TestConfig.SdkType);
        
        if (_eventResourceAdapter != null)
        {
            var info = _eventResourceAdapter.GetAdapterInfo();
            _logger.Log($"Event Resource Adapter: {info.SdkType} v{info.Version} - Initialized: {info.IsInitialized}");
        }
        
        return _eventResourceAdapter;
    }
    
    protected async Task<IWorkflowResourceAdapter> GetWorkflowResourceAdapterAsync()
    { 
        _workflowResourceAdapter ??= await AdapterFactory.CreateWorkflowResourceAdapterAsync(TestConfig.SdkType);
        
        if (_workflowResourceAdapter != null)
        {
            var info = _workflowResourceAdapter.GetAdapterInfo();
            _logger.Log($"Workflow Resource Adapter: {info.SdkType} v{info.Version} - Initialized: {info.IsInitialized}");
        }
        
        return _workflowResourceAdapter;
    }
    
    protected async Task<bool> ValidateSdkResponseAsync<T>(SdkResponse<T> sdkResponse, RestResponse<T> apiResponse)
    {
        return await _responseComparer.CompareAsync(sdkResponse, apiResponse);
    }
    
    public virtual void Dispose()
    {
        _eventResourceAdapter?.Dispose();
        _workflowResourceAdapter?.Dispose();
        _logger.Log($"Test '{TestContext.Current.TestCase?.TestCaseDisplayName}' completed.");
    }
} 