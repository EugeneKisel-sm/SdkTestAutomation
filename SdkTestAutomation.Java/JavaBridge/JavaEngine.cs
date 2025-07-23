using System;
using System.Reflection;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Java.JavaBridge;

/// <summary>
/// Optimized Java bridge engine using IKVM.NET
/// </summary>
public class JavaEngine : IDisposable
{
    private bool _disposed = false;
    private dynamic? _conductorClient;
    private dynamic? _eventClient;
    
    /// <summary>
    /// Initialize the Java bridge using IKVM
    /// </summary>
    public void Initialize(AdapterConfiguration config)
    {
        try
        {
            // Create Conductor client using IKVM
            CreateConductorClient(config.ServerUrl);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Java bridge: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Create Conductor client using IKVM
    /// </summary>
    private void CreateConductorClient(string serverUrl)
    {
        try
        {
            // Create ConductorClient using IKVM - these are now .NET types
            var conductorClientType = Type.GetType("com.netflix.conductor.client.http.ConductorClient, conductor-client");
            if (conductorClientType == null)
            {
                throw new InvalidOperationException("ConductorClient class not found. Ensure conductor-client.jar is in the lib folder and properly referenced.");
            }
            
            _conductorClient = Activator.CreateInstance(conductorClientType, serverUrl);
            
            // Create EventClient using IKVM
            var eventClientType = Type.GetType("com.netflix.conductor.client.http.EventClient, conductor-client");
            if (eventClientType == null)
            {
                throw new InvalidOperationException("EventClient class not found. Ensure conductor-client.jar is in the lib folder and properly referenced.");
            }
            
            _eventClient = Activator.CreateInstance(eventClientType, _conductorClient);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Conductor client: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Get event handlers using Java SDK
    /// </summary>
    public dynamic GetEventHandlers(string eventName = "", bool activeOnly = false)
    {
        if (_eventClient == null)
        {
            throw new InvalidOperationException("EventClient not initialized");
        }
        
        return _eventClient.getEventHandlers(eventName, activeOnly);
    }
    
    /// <summary>
    /// Register event handler using Java SDK
    /// </summary>
    public dynamic RegisterEventHandler(dynamic eventHandler)
    {
        if (_eventClient == null)
        {
            throw new InvalidOperationException("EventClient not initialized");
        }
        
        return _eventClient.registerEventHandler(eventHandler);
    }
    
    /// <summary>
    /// Update event handler using Java SDK
    /// </summary>
    public dynamic UpdateEventHandler(dynamic eventHandler)
    {
        if (_eventClient == null)
        {
            throw new InvalidOperationException("EventClient not initialized");
        }
        
        return _eventClient.updateEventHandler(eventHandler);
    }
    
    /// <summary>
    /// Unregister event handler using Java SDK
    /// </summary>
    public dynamic UnregisterEventHandler(string handlerName)
    {
        if (_eventClient == null)
        {
            throw new InvalidOperationException("EventClient not initialized");
        }
        
        return _eventClient.unregisterEventHandler(handlerName);
    }
    
    /// <summary>
    /// Create a Java object instance using IKVM
    /// </summary>
    public dynamic CreateInstance(string className, params object[] args)
    {
        try
        {
            // Try to find the type in the appropriate assembly
            Type? type = FindType(className);
            if (type == null)
            {
                throw new InvalidOperationException($"Class {className} not found in any referenced assembly");
            }
            
            return Activator.CreateInstance(type, args);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create instance of {className}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Find a type in the referenced assemblies
    /// </summary>
    private Type? FindType(string className)
    {
        var assemblies = new[] { "conductor-client", "conductor-common", "IKVM" };
        
        foreach (var assembly in assemblies)
        {
            var type = Type.GetType($"{className}, {assembly}");
            if (type != null) return type;
        }
        
        return null;
    }
    
    /// <summary>
    /// Dispose the Java bridge
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _conductorClient = null;
            _eventClient = null;
            _disposed = true;
        }
    }
} 