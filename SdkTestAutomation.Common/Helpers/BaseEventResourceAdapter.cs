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
    protected AdapterConfiguration? _config;
    
    protected BaseEventResourceAdapter()
    {
        _logger = new ConsoleLogger(null);
    }
    
    public abstract string SdkType { get; }
    
    public abstract Task<bool> InitializeAsync(AdapterConfiguration config);
    
    public abstract Task<bool> IsHealthyAsync();
    
    public virtual AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo
        {
            SdkType = SdkType,
            Version = GetSdkVersion(),
            IsInitialized = IsInitialized(),
            InitializedAt = DateTime.UtcNow,
            ErrorMessage = IsInitialized() ? null : "Not initialized"
        };
    }
    
    public abstract Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request);
    
    public abstract Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request);
    
    public abstract void Dispose();
    
    /// <summary>
    /// Get the SDK version - override in derived classes
    /// </summary>
    protected virtual string GetSdkVersion() => "1.0.0";
    
    /// <summary>
    /// Check if the adapter is initialized - override in derived classes
    /// </summary>
    protected abstract bool IsInitialized();
    
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
    protected void LogOperation(string operation, string? details = null)
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
} 