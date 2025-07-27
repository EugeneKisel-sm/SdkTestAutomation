using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public class GoSharedLibraryClient : ISdkClient
{
    private bool _initialized;
    private string _serverUrl;
    private IntPtr _clientHandle;
    
    public bool IsInitialized => _initialized && _clientHandle != IntPtr.Zero;
    
    // P/Invoke declarations for Go functions
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreateConductorClient([MarshalAs(UnmanagedType.LPStr)] string serverUrl);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DestroyConductorClient(IntPtr client);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr AddEventHandler(IntPtr client, 
        [MarshalAs(UnmanagedType.LPStr)] string name,
        [MarshalAs(UnmanagedType.LPStr)] string eventType, 
        bool active);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetEvents(IntPtr client);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetEventByName(IntPtr client, 
        [MarshalAs(UnmanagedType.LPStr)] string eventName);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr UpdateEvent(IntPtr client,
        [MarshalAs(UnmanagedType.LPStr)] string name,
        [MarshalAs(UnmanagedType.LPStr)] string eventType, 
        bool active);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr DeleteEvent(IntPtr client, 
        [MarshalAs(UnmanagedType.LPStr)] string name);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StartWorkflow(IntPtr client,
        [MarshalAs(UnmanagedType.LPStr)] string name,
        int version,
        [MarshalAs(UnmanagedType.LPStr)] string correlationId);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetWorkflow(IntPtr client, 
        [MarshalAs(UnmanagedType.LPStr)] string workflowId);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetWorkflows(IntPtr client);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr TerminateWorkflow(IntPtr client,
        [MarshalAs(UnmanagedType.LPStr)] string workflowId,
        [MarshalAs(UnmanagedType.LPStr)] string reason);
    
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FreeString(IntPtr ptr);
    
    public void Initialize(string serverUrl)
    {
        try
        {
            _serverUrl = serverUrl;
            _clientHandle = CreateConductorClient(serverUrl);
            
            if (_clientHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create Go conductor client");
            }
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Go shared library client: {ex.Message}", ex);
        }
    }
    
    public string ExecuteGoCall(string method, object requestData = null)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Go client not initialized");
        }
        
        try
        {
            IntPtr resultPtr = IntPtr.Zero;
            
            switch (method)
            {
                case "AddEvent":
                    var addEventData = JsonSerializer.Deserialize<AddEventRequest>(JsonSerializer.Serialize(requestData));
                    resultPtr = AddEventHandler(_clientHandle, addEventData.Name, addEventData.Event, addEventData.Active);
                    break;
                    
                case "GetEvents":
                    resultPtr = GetEvents(_clientHandle);
                    break;
                    
                case "GetEventByName":
                    var getEventData = JsonSerializer.Deserialize<GetEventByNameRequest>(JsonSerializer.Serialize(requestData));
                    resultPtr = GetEventByName(_clientHandle, getEventData.EventName);
                    break;
                    
                case "UpdateEvent":
                    var updateEventData = JsonSerializer.Deserialize<UpdateEventRequest>(JsonSerializer.Serialize(requestData));
                    resultPtr = UpdateEvent(_clientHandle, updateEventData.Name, updateEventData.Event, updateEventData.Active);
                    break;
                    
                case "DeleteEvent":
                    var deleteEventData = JsonSerializer.Deserialize<DeleteEventRequest>(JsonSerializer.Serialize(requestData));
                    resultPtr = DeleteEvent(_clientHandle, deleteEventData.Name);
                    break;
                    
                case "StartWorkflow":
                    var startWorkflowData = JsonSerializer.Deserialize<StartWorkflowRequest>(JsonSerializer.Serialize(requestData));
                    resultPtr = StartWorkflow(_clientHandle, startWorkflowData.Name, startWorkflowData.Version, startWorkflowData.CorrelationId);
                    break;
                    
                case "GetWorkflow":
                    var getWorkflowData = JsonSerializer.Deserialize<GetWorkflowRequest>(JsonSerializer.Serialize(requestData));
                    resultPtr = GetWorkflow(_clientHandle, getWorkflowData.WorkflowId);
                    break;
                    
                case "GetWorkflows":
                    resultPtr = GetWorkflows(_clientHandle);
                    break;
                    
                case "TerminateWorkflow":
                    var terminateWorkflowData = JsonSerializer.Deserialize<TerminateWorkflowRequest>(JsonSerializer.Serialize(requestData));
                    resultPtr = TerminateWorkflow(_clientHandle, terminateWorkflowData.WorkflowId, terminateWorkflowData.Reason);
                    break;
                    
                default:
                    throw new ArgumentException($"Unknown method: {method}");
            }
            
            if (resultPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Go call failed for method: {method}");
            }
            
            var result = Marshal.PtrToStringAnsi(resultPtr);
            FreeString(resultPtr);
            
            return result ?? "{}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to execute Go call '{method}': {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        if (_clientHandle != IntPtr.Zero)
        {
            DestroyConductorClient(_clientHandle);
            _clientHandle = IntPtr.Zero;
        }
        _initialized = false;
    }
    
    // Request models for type safety
    private class AddEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }
    
    private class GetEventByNameRequest
    {
        public string EventName { get; set; } = string.Empty;
    }
    
    private class UpdateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }
    
    private class DeleteEventRequest
    {
        public string Name { get; set; } = string.Empty;
    }
    
    private class StartWorkflowRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public string CorrelationId { get; set; } = string.Empty;
    }
    
    private class GetWorkflowRequest
    {
        public string WorkflowId { get; set; } = string.Empty;
    }
    
    private class TerminateWorkflowRequest
    {
        public string WorkflowId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
} 