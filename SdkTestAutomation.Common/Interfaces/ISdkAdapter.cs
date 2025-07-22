using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Interfaces;

/// <summary>
/// Base interface for all SDK adapters
/// </summary>
public interface ISdkAdapter : IDisposable
{
    /// <summary>
    /// The type of SDK this adapter represents
    /// </summary>
    string SdkType { get; }
    
    /// <summary>
    /// Initialize the adapter with configuration
    /// </summary>
    Task<bool> InitializeAsync(AdapterConfiguration config);
    
    /// <summary>
    /// Check if the adapter is healthy and ready to use
    /// </summary>
    Task<bool> IsHealthyAsync();
    
    /// <summary>
    /// Get adapter information
    /// </summary>
    AdapterInfo GetAdapterInfo();
} 