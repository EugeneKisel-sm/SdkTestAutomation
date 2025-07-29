using System.Runtime.InteropServices;
using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public class GoClient : ISdkClient
{
    private bool _initialized;
    private string _serverUrl;
    private int _clientHandle;
    
    public bool IsInitialized => _initialized && _clientHandle != 0;
    
    // P/Invoke declarations for Go functions
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern int CreateConductorClient([MarshalAs(UnmanagedType.LPStr)] string serverUrl);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void DestroyConductorClient(int clientId);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr AddEventHandler(int clientId, 
        [MarshalAs(UnmanagedType.LPStr)] string name,
        [MarshalAs(UnmanagedType.LPStr)] string eventType, 
        bool active);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr GetEvents(int clientId);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr GetEventByName(int clientId, 
        [MarshalAs(UnmanagedType.LPStr)] string eventName);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr UpdateEvent(int clientId,
        [MarshalAs(UnmanagedType.LPStr)] string name,
        [MarshalAs(UnmanagedType.LPStr)] string eventType, 
        bool active);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr DeleteEvent(int clientId, 
        [MarshalAs(UnmanagedType.LPStr)] string name);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr StartWorkflow(int clientId,
        [MarshalAs(UnmanagedType.LPStr)] string name,
        int version,
        [MarshalAs(UnmanagedType.LPStr)] string correlationId);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr GetWorkflow(int clientId, 
        [MarshalAs(UnmanagedType.LPStr)] string workflowId);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr GetWorkflows(int clientId);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern IntPtr TerminateWorkflow(int clientId,
        [MarshalAs(UnmanagedType.LPStr)] string workflowId,
        [MarshalAs(UnmanagedType.LPStr)] string reason);
    
#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl)]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void FreeString(IntPtr ptr);

#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLogs")]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLogs")]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLogs")]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetLogs")]
#endif
    private static extern IntPtr GetLogsInternal();

#if WINDOWS
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ClearLogs")]
#elif OSX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dylib", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ClearLogs")]
#elif LINUX
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.so", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ClearLogs")]
#else
    [DllImport("Implementations/Go/build-artifacts/conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ClearLogs")]
#endif
    private static extern void ClearLogsInternal();
    
    public void Initialize(string serverUrl)
    {
        try
        {
            _serverUrl = serverUrl;
            _clientHandle = CreateConductorClient(serverUrl);
            
            if (_clientHandle == 0)
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
            Console.WriteLine($"[C#] ExecuteGoCall: {method} with client ID: {_clientHandle}");
            IntPtr resultPtr = IntPtr.Zero;
            
            switch (method)
            {
                case "AddEvent":
                    var addEventData = JsonSerializer.Deserialize<AddEventRequest>(JsonSerializer.Serialize(requestData));
                    Console.WriteLine($"[C#] Calling AddEventHandler with name: {addEventData.Name}, event: {addEventData.Event}, active: {addEventData.Active}");
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
                Console.WriteLine($"[C#] ERROR: Go call returned null pointer for method: {method}");
                throw new InvalidOperationException($"Go call failed for method: {method}");
            }
            
            var result = Marshal.PtrToStringAnsi(resultPtr);
            Console.WriteLine($"[C#] Go call result: {result}");
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
        if (_clientHandle != 0)
        {
            DestroyConductorClient(_clientHandle);
            _clientHandle = 0;
        }
    }

    public string GetLogs()
    {
        try
        {
            IntPtr logsPtr = GetLogsInternal();
            if (logsPtr == IntPtr.Zero)
            {
                return string.Empty;
            }
            
            var logs = Marshal.PtrToStringAnsi(logsPtr);
            FreeString(logsPtr);
            return logs ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[C#] Error getting logs: {ex.Message}");
            return string.Empty;
        }
    }

    public void ClearLogs()
    {
        try
        {
            ClearLogsInternal();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[C#] Error clearing logs: {ex.Message}");
        }
    }
    
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