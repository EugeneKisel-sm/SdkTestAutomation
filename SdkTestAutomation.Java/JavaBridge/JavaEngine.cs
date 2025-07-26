using System;
using System.Reflection;
using SdkTestAutomation.Sdk.Models;

namespace SdkTestAutomation.Java.JavaBridge;

/// <summary>
/// Optimized Java bridge engine using IKVM.NET
/// </summary>
public class JavaEngine : IDisposable
{
    private bool _disposed = false;
    private dynamic _conductorClient;
    private dynamic _eventClient;
    
    /// <summary>
    /// Initialize the Java bridge using IKVM
    /// </summary>
    public void Initialize(AdapterConfiguration config)
    {
        CreateConductorClient(config.ServerUrl);
    }
    
    /// <summary>
    /// Create Conductor client using IKVM
    /// </summary>
    private void CreateConductorClient(string serverUrl)
    {
        _conductorClient = CreateJavaObject("com.netflix.conductor.client.http.ConductorClient", serverUrl);
        _eventClient = CreateJavaObject("com.netflix.conductor.client.http.EventClient", _conductorClient);
    }
    
    /// <summary>
    /// Create Java object using IKVM's type system
    /// </summary>
    private dynamic CreateJavaObject(string className, params object[] args)
    {
        var type = Type.GetType($"{className}, conductor-client") ?? 
                  Type.GetType($"{className}, conductor-common") ?? 
                  Type.GetType($"{className}, IKVM");
        
        if (type == null)
        {
            throw new InvalidOperationException($"Could not find type {className} in any referenced assembly");
        }
        
        return Activator.CreateInstance(type, args);
    }
    
    /// <summary>
    /// Get the Conductor client instance
    /// </summary>
    public dynamic GetConductorClient()
    {
        if (_conductorClient == null)
        {
            throw new InvalidOperationException("ConductorClient not initialized");
        }
        
        return _conductorClient;
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
        return CreateJavaObject(className, args);
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