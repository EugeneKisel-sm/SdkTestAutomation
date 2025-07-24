using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Base class for event resource adapters to reduce code duplication
/// </summary>
public abstract class BaseEventResourceAdapter : IEventResourceAdapter
{
    protected readonly ILogger _logger;
    protected AdapterConfiguration _config;
    
    protected BaseEventResourceAdapter()
    {
        _logger = new ConsoleLogger(null);
    }
    
    public abstract string SdkType { get; }
    
    public virtual async Task<bool> InitializeAsync(AdapterConfiguration config)
    {
        try
        {
            _config = config;
            LogOperation($"Initializing {SdkType} SDK adapter", config.ServerUrl);
            
            await InitializeEngineAsync(config);
            
            LogOperation($"{SdkType} SDK adapter initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            LogError($"initializing {SdkType} SDK adapter", ex);
            return false;
        }
    }
    
    public virtual Task<bool> IsHealthyAsync()
    {
        if (!IsInitialized()) return Task.FromResult(false);
        
        return Task.Run(() => 
        {
            try
            {
                PerformHealthCheck();
                return true;
            }
            catch
            {
                return false;
            }
        });
    }
    
    public virtual AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo
        {
            SdkType = SdkType,
            Version = GetSdkVersion(),
            IsInitialized = IsInitialized(),
            InitializedAt = DateTime.UtcNow,
            ErrorMessage = IsInitialized() ? string.Empty : "Not initialized"
        };
    }
    
    public abstract Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request);
    
    public virtual void Dispose()
    {
        try
        {
            DisposeEngine();
        }
        catch (Exception ex)
        {
            LogError($"disposing {SdkType} adapter", ex);
        }
    }
    
    /// <summary>
    /// Dispose the engine - override in derived classes
    /// </summary>
    protected abstract void DisposeEngine();
    
    /// <summary>
    /// Get the SDK version - override in derived classes
    /// </summary>
    protected virtual string GetSdkVersion() => "1.0.0";
    
    /// <summary>
    /// Check if the adapter is initialized - override in derived classes
    /// </summary>
    protected abstract bool IsInitialized();
    
    /// <summary>
    /// Initialize the engine - override in derived classes
    /// </summary>
    protected abstract Task InitializeEngineAsync(AdapterConfiguration config);
    
    /// <summary>
    /// Perform health check - override in derived classes
    /// </summary>
    protected abstract void PerformHealthCheck();
    
    /// <summary>
    /// Validate that the adapter is initialized
    /// </summary>
    protected void ValidateInitialization()
    {
        if (!IsInitialized())
            throw new InvalidOperationException($"{SdkType} adapter not initialized");
    }
    
    /// <summary>
    /// Log operation start
    /// </summary>
    protected void LogOperation(string operation, string details = null)
    {
        var message = details != null ? $"{operation}: {details}" : operation;
        _logger.Log(message);
    }
    
    /// <summary>
    /// Log error with operation context
    /// </summary>
    protected void LogError(string operation, Exception ex)
    {
        _logger.Log($"Error {operation}: {ex.Message}");
    }
    
    /// <summary>
    /// Execute operation with standardized error handling
    /// </summary>
    protected async Task<SdkResponse<GetEventResponse>> ExecuteOperationAsync(
        string operation, 
        Func<Task<SdkResponse<GetEventResponse>>> operationFunc,
        Func<Exception, int> getStatusCode = null)
    {
        try
        {
            ValidateInitialization();
            LogOperation(operation);
            return await operationFunc();
        }
        catch (Exception ex)
        {
            LogError(operation, ex);
            var statusCode = getStatusCode?.Invoke(ex) ?? 500;
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, statusCode);
        }
    }
    
    /// <summary>
    /// Create success response with events data
    /// </summary>
    protected SdkResponse<GetEventResponse> CreateSuccessResponseWithEvents(dynamic events, string mapperMethod)
    {
        var eventsList = mapperMethod switch
        {
            "MapCSharpCollection" => EventInfoMapper.MapCSharpCollection(events),
            "MapJavaCollection" => EventInfoMapper.MapJavaCollection(events),
            "MapPythonCollection" => EventInfoMapper.MapPythonCollection(events),
            _ => throw new ArgumentException($"Unknown mapper method: {mapperMethod}")
        };
        
        var data = new GetEventResponse
        {
            Events = eventsList
        };
        return SdkResponseBuilder.CreateSuccessResponse(data);
    }
    
    /// <summary>
    /// Get API exception status code or default - override in derived classes
    /// </summary>
    protected virtual int GetApiExceptionStatusCode(Exception ex, int defaultCode = 500)
    {
        return defaultCode;
    }
} 