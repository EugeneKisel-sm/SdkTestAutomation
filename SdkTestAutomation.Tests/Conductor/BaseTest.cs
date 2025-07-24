using SdkTestAutomation.Api.Conductor.EventResource;
using SdkTestAutomation.Api.Conductor.WorkflowResource;
using SdkTestAutomation.Common.Helpers;
using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;
using RestSharp;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

/// <summary>
/// Base test class for all Conductor SDK integration tests
/// </summary>
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
        
        _logger.Log($"Using SDK type: {TestConfig.SdkType}");
        
        _responseComparer = new ResponseComparer(_logger);
        _logger.Log($"Test '{testContext.TestCase.TestCaseDisplayName}' execution started.");
        
        EventResourceApi = new EventResourceApi(_logger);
        WorkflowResourceApi = new WorkflowResourceApi(_logger);
    }
    
    /// <summary>
    /// Get the event resource adapter for the selected SDK
    /// </summary>
    protected async Task<IEventResourceAdapter> GetEventResourceAdapterAsync()
    {
        if (_eventResourceAdapter == null)
        {
            _eventResourceAdapter = await AdapterFactory.CreateEventResourceAdapterAsync(TestConfig.SdkType);
        }
        return _eventResourceAdapter;
    }
    
    /// <summary>
    /// Get the workflow resource adapter for the selected SDK
    /// </summary>
    protected async Task<IWorkflowResourceAdapter> GetWorkflowResourceAdapterAsync()
    {
        if (_workflowResourceAdapter == null)
        {
            _workflowResourceAdapter = await AdapterFactory.CreateWorkflowResourceAdapterAsync(TestConfig.SdkType);
        }
        return _workflowResourceAdapter;
    }
    
    /// <summary>
    /// Execute an SDK call and validate it against the direct API response
    /// </summary>
    protected async Task<bool> ValidateSdkResponseAsync<T>(SdkResponse<T> sdkResponse, RestResponse<T> apiResponse)
    {
        return await _responseComparer.CompareAsync(sdkResponse, apiResponse);
    }
    
    /// <summary>
    /// Log adapter information
    /// </summary>
    protected void LogAdapterInfo()
    {
        if (_eventResourceAdapter != null)
        {
            var info = _eventResourceAdapter.GetAdapterInfo();
            _logger.Log($"Event Resource Adapter: {info.SdkType} v{info.Version} - Initialized: {info.IsInitialized}");
        }
        
        if (_workflowResourceAdapter != null)
        {
            var info = _workflowResourceAdapter.GetAdapterInfo();
            _logger.Log($"Workflow Resource Adapter: {info.SdkType} v{info.Version} - Initialized: {info.IsInitialized}");
        }
    }
    
    public virtual void Dispose()
    {
        _eventResourceAdapter?.Dispose();
        _workflowResourceAdapter?.Dispose();
        _logger.Log($"Test '{TestContext.Current.TestCase.TestCaseDisplayName}' completed with {TestContext.Current.TestStatus}.");
    }
} 