# Go SDK Integration Approaches Comparison

## Overview

This document compares different approaches for integrating the [conductor-oss/go-sdk](https://github.com/conductor-oss/go-sdk) into the SdkTestAutomation framework.

## 🔍 **Previous Approach: CLI/Process Communication (Replaced)**

### **What We Had**
```csharp
// Creates temporary Go files and runs them
var goCode = CreateAddEventGoCode(name, eventType, active);
var tempFile = CreateTempGoFile(goCode);
var result = _client.ExecuteGoCommand(tempFile); // "go run tempfile.go"
```

### **Pros**
- ✅ **Simple Implementation**: Easy to understand and implement
- ✅ **No Dependencies**: Only requires Go installation
- ✅ **Isolation**: Each operation runs in separate process
- ✅ **Flexibility**: Can use any Go features and libraries

### **Cons**
- ❌ **Performance Overhead**: Each operation compiles and runs Go code
- ❌ **Temporary Files**: Creates files for every operation
- ❌ **Security Concerns**: File creation and execution
- ❌ **Reliability Issues**: Process startup/teardown for each call
- ❌ **Resource Usage**: High CPU and memory usage

## 🚀 **Alternative Approaches**

### **1. HTTP API Bridge (Recommended) - IMPLEMENTED**

#### **Implementation**
```csharp
public class GoHttpClient : ISdkClient
{
    public async Task<string> ExecuteGoApiCallAsync(string endpoint, object requestData = null)
    {
        var url = $"{_goApiUrl}/{endpoint}";
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        
        if (requestData != null)
        {
            var json = JsonSerializer.Serialize(requestData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
        
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}
```

#### **Go API Server**
```go
package main

import (
    "net/http"
    "github.com/gorilla/mux"
    "github.com/conductor-sdk/conductor-go/sdk/client"
)

func main() {
    apiClient := client.NewAPIClientFromEnv()
    r := mux.NewRouter()
    
    // Event operations
    r.HandleFunc("/events/add", addEventHandler).Methods("POST")
    r.HandleFunc("/events/get", getEventsHandler).Methods("POST")
    
    // Workflow operations
    r.HandleFunc("/workflows/start", startWorkflowHandler).Methods("POST")
    
    log.Fatal(http.ListenAndServe(":8081", r))
}
```

#### **Pros**
- ✅ **Better Performance**: Single server process, no compilation overhead
- ✅ **Reusable**: Server stays running between operations
- ✅ **Standard Protocol**: HTTP is well-understood and debuggable
- ✅ **Async Support**: Natural async/await support
- ✅ **Error Handling**: Standard HTTP error codes

#### **Cons**
- ⚠️ **Setup Complexity**: Requires starting and managing server process
- ⚠️ **Network Overhead**: HTTP communication layer
- ⚠️ **Port Management**: Need to handle port conflicts

### **2. gRPC Bridge (High Performance)**

#### **Implementation**
```csharp
public class GoGrpcClient : ISdkClient
{
    public async Task<string> ExecuteGrpcCallAsync(string method, object requestData = null)
    {
        // Use gRPC client to communicate with Go server
        // Much more efficient than HTTP
    }
}
```

#### **Go gRPC Server**
```go
package main

import (
    "google.golang.org/grpc"
    pb "path/to/generated/proto"
)

type ConductorService struct {
    pb.UnimplementedConductorServiceServer
    apiClient *client.APIClient
}

func (s *ConductorService) AddEventHandler(ctx context.Context, req *pb.AddEventRequest) (*pb.AddEventResponse, error) {
    // Implementation
}
```

#### **Pros**
- ✅ **High Performance**: Binary protocol, efficient serialization
- ✅ **Type Safety**: Strongly typed contracts
- ✅ **Streaming Support**: Can handle streaming operations
- ✅ **Code Generation**: Automatic client/server code generation
- ✅ **Interoperability**: Works across many languages

#### **Cons**
- ⚠️ **Complexity**: Requires protobuf definitions and code generation
- ⚠️ **Learning Curve**: More complex than HTTP
- ⚠️ **Setup Overhead**: Need to manage protobuf compilation

### **3. Shared Library Approach (Best Performance)**

#### **Implementation**
```csharp
public class GoSharedLibraryClient : ISdkClient
{
    [DllImport("conductor-go-bridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr AddEventHandler(IntPtr client, string name, string eventType, bool active);
    
    public string AddEvent(string name, string eventType, bool active)
    {
        return ExecuteSharedLibraryCall(client => AddEventHandler(client, name, eventType, active));
    }
}
```

#### **Go Shared Library**
```go
package main

import "C"

//export AddEventHandler
func AddEventHandler(client C.void*, name *C.char, eventType *C.char, active C.int) *C.char {
    // Implementation using conductor-go SDK
    // Return JSON string
}

func main() {}
```

#### **Pros**
- ✅ **Best Performance**: Direct function calls, no network overhead
- ✅ **Low Latency**: Minimal overhead between .NET and Go
- ✅ **Memory Efficient**: No process creation or network communication
- ✅ **Type Safety**: Direct parameter passing

#### **Cons**
- ❌ **Platform Specific**: Different libraries for different platforms
- ❌ **Complex Build**: Requires CGO and cross-compilation
- ❌ **Memory Management**: Manual memory management between languages
- ❌ **Debugging Difficulty**: Harder to debug cross-language calls

### **4. WebAssembly (Modern Alternative)**

#### **Implementation**
```csharp
public class GoWasmClient : ISdkClient
{
    public async Task<string> ExecuteWasmCallAsync(string method, object requestData = null)
    {
        // Use .NET WebAssembly runtime to execute Go WASM
        // Modern approach with good performance
    }
}
```

#### **Go WebAssembly**
```go
package main

import (
    "syscall/js"
    "github.com/conductor-sdk/conductor-go/sdk/client"
)

func addEventHandler(this js.Value, args []js.Value) interface{} {
    // Implementation using conductor-go SDK
    return js.ValueOf(result)
}

func main() {
    js.Global().Set("addEventHandler", js.FuncOf(addEventHandler))
    select {}
}
```

#### **Pros**
- ✅ **Cross-Platform**: Same binary works everywhere
- ✅ **Security**: Sandboxed execution environment
- ✅ **Modern**: Leverages WebAssembly technology
- ✅ **Performance**: Good performance characteristics

#### **Cons**
- ⚠️ **Maturity**: WASM support in Go is still evolving
- ⚠️ **Complexity**: Requires WASM runtime and toolchain
- ⚠️ **Size**: WASM binaries can be large

## 📊 **Performance Comparison**

| Approach | Latency | Throughput | Memory Usage | Setup Complexity |
|----------|---------|------------|--------------|------------------|
| **CLI/Process** | High | Low | High | Low |
| **HTTP API** | Medium | Medium | Medium | Medium |
| **gRPC** | Low | High | Low | High |
| **Shared Library** | Very Low | Very High | Very Low | Very High |
| **WebAssembly** | Low | Medium | Low | High |

## 🎯 **Recommendations**

### **For Development/Testing**
**HTTP API Bridge** - Best balance of performance and simplicity
- Easy to implement and debug
- Good performance for testing scenarios
- Standard HTTP tooling and monitoring

### **For Production**
**gRPC Bridge** - Best performance with reasonable complexity
- High performance for production workloads
- Type safety and code generation
- Good tooling and monitoring support

### **For Maximum Performance**
**Shared Library** - If you can handle the complexity
- Best performance characteristics
- Requires significant build infrastructure
- Platform-specific deployment

### **For Modern Applications**
**WebAssembly** - Future-proof approach
- Cross-platform compatibility
- Modern technology stack
- Good performance characteristics

## 🔧 **Implementation Strategy**

### **Phase 1: HTTP API Bridge (Immediate) - COMPLETED**
1. ✅ Implement HTTP API bridge approach
2. ✅ Replace current CLI approach
3. ✅ Improve performance and reliability

### **Phase 2: gRPC Bridge (Medium-term)**
1. Add gRPC support alongside HTTP
2. Provide performance improvements
3. Maintain backward compatibility

### **Phase 3: Shared Library (Long-term)**
1. Build shared library infrastructure
2. Provide maximum performance option
3. Platform-specific optimizations

## 📝 **Code Examples**

### **HTTP API Bridge Usage**
```csharp
var goClient = new GoHttpClient();
goClient.Initialize("http://localhost:8080/api");

var response = await goClient.ExecuteGoApiCallAsync("events/add", new {
    name = "test_event",
    event = "test_type",
    active = true
});
```

### **gRPC Bridge Usage**
```csharp
var goClient = new GoGrpcClient();
goClient.Initialize("http://localhost:8080/api");

var response = await goClient.ExecuteGrpcCallAsync("AddEventHandler", new {
    name = "test_event",
    event = "test_type",
    active = true
});
```

### **Shared Library Usage**
```csharp
var goClient = new GoSharedLibraryClient();
goClient.Initialize("http://localhost:8080/api");

var response = goClient.AddEvent("test_event", "test_type", true);
```

## ✅ **Conclusion**

The **HTTP API Bridge** approach has been implemented as the replacement for the CLI approach. It provides:

- ✅ **Significant Performance Improvement**: No compilation overhead
- ✅ **Better Reliability**: Single server process
- ✅ **Easier Debugging**: Standard HTTP protocol
- ✅ **Reasonable Complexity**: Manageable implementation effort

**Status**: ✅ **IMPLEMENTED** - The HTTP API Bridge is now the active Go SDK integration method.

For production environments, consider migrating to **gRPC** for better performance, or **Shared Library** for maximum performance if you can handle the complexity. 