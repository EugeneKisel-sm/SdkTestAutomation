using SdkTestAutomation.Api.Conductor.EventResource;
using SdkTestAutomation.Api.Conductor.WorkflowResource;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;
using RestSharp;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public abstract class BaseTest : IDisposable
{
    private readonly ILogger _logger;
    private readonly SdkCommandExecutor _sdkCommandExecutor;
    private readonly ResponseComparer _responseComparer;

    protected EventResourceApi EventResourceApi { get; }
    protected WorkflowResourceApi WorkflowResourceApi { get; }
    
    protected BaseTest()
    {
        var testContext = TestContext.Current;
        _logger = new ConsoleLogger(testContext);
        
        _sdkCommandExecutor = new SdkCommandExecutor(_logger);
        _responseComparer = new ResponseComparer(_logger);
        
        _logger.Log($"Test '{testContext.TestCase.TestCaseDisplayName}' execution started.");
        
        EventResourceApi = new EventResourceApi(_logger);
        WorkflowResourceApi = new WorkflowResourceApi(_logger);
    }
    
    /// <summary>
    /// Executes a call to the selected SDK via CLI wrapper
    /// </summary>
    protected Task<SdkResponse<T>> ExecuteSdkCallAsync<T>(string operation, Dictionary<string, object> parameters, string resource)
        => _sdkCommandExecutor.ExecuteAsync<T>(operation, parameters, resource);
    
    /// <summary>
    /// Validates that SDK response matches the direct API response
    /// </summary>
    protected Task<bool> ValidateSdkResponseAsync<T>(SdkResponse<T> sdkResponse, RestResponse<T> apiResponse)
        => _responseComparer.CompareAsync(sdkResponse, apiResponse);

    public virtual void Dispose()
    {
        _logger.Log($"Test '{TestContext.Current.TestCase.TestCaseDisplayName}' completed with {TestContext.Current.TestStatus}.");
    }
} 