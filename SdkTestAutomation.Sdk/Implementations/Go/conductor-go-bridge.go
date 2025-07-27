package main

/*
#cgo CFLAGS: -I${SRCDIR}
#cgo LDFLAGS: -L${SRCDIR} -lstdc++
#include <stdlib.h>
#include <stdint.h>

// C function declarations
extern void* CreateConductorClient(const char* serverUrl);
extern void DestroyConductorClient(void* client);
extern char* AddEventHandler(void* client, const char* name, const char* eventType, int active);
extern char* GetEvents(void* client);
extern char* GetEventByName(void* client, const char* eventName);
extern char* UpdateEvent(void* client, const char* name, const char* eventType, int active);
extern char* DeleteEvent(void* client, const char* name);
extern char* StartWorkflow(void* client, const char* name, int version, const char* correlationId);
extern char* GetWorkflow(void* client, const char* workflowId);
extern char* GetWorkflows(void* client);
extern char* TerminateWorkflow(void* client, const char* workflowId, const char* reason);
extern void FreeString(char* ptr);
*/
import "C"
import (
    "encoding/json"
    "fmt"
    "unsafe"
    
    "github.com/conductor-sdk/conductor-go/sdk/client"
    "github.com/conductor-sdk/conductor-go/sdk/model"
)

// Global client storage
var clients = make(map[unsafe.Pointer]*client.APIClient)

//export CreateConductorClient
func CreateConductorClient(serverUrl *C.char) unsafe.Pointer {
    goServerUrl := C.GoString(serverUrl)
    
    // Create API client
    apiClient := client.NewAPIClientFromEnv()
    
    // Store client reference
    clientPtr := unsafe.Pointer(apiClient)
    clients[clientPtr] = apiClient
    
    return clientPtr
}

//export DestroyConductorClient
func DestroyConductorClient(client unsafe.Pointer) {
    if apiClient, exists := clients[client]; exists {
        // Clean up client
        delete(clients, client)
        // Note: Go garbage collector will handle the rest
    }
}

//export AddEventHandler
func AddEventHandler(client unsafe.Pointer, name *C.char, eventType *C.char, active C.int) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goName := C.GoString(name)
    goEventType := C.GoString(eventType)
    goActive := active != 0
    
    eventHandler := &model.EventHandler{
        Name:   goName,
        Event:  goEventType,
        Active: goActive,
        Actions: []model.EventAction{},
    }
    
    err := apiClient.EventResourceApi.RegisterEventHandler(eventHandler)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Event handler registered successfully",
    }
    jsonResponse, _ := json.Marshal(response)
    return C.CString(string(jsonResponse))
}

//export GetEvents
func GetEvents(client unsafe.Pointer) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    events, err := apiClient.EventResourceApi.GetEventHandlers("", false)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    jsonResponse, _ := json.Marshal(events)
    return C.CString(string(jsonResponse))
}

//export GetEventByName
func GetEventByName(client unsafe.Pointer, eventName *C.char) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goEventName := C.GoString(eventName)
    events, err := apiClient.EventResourceApi.GetEventHandlers(goEventName, false)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    jsonResponse, _ := json.Marshal(events)
    return C.CString(string(jsonResponse))
}

//export UpdateEvent
func UpdateEvent(client unsafe.Pointer, name *C.char, eventType *C.char, active C.int) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goName := C.GoString(name)
    goEventType := C.GoString(eventType)
    goActive := active != 0
    
    eventHandler := &model.EventHandler{
        Name:   goName,
        Event:  goEventType,
        Active: goActive,
        Actions: []model.EventAction{},
    }
    
    err := apiClient.EventResourceApi.UpdateEventHandler(eventHandler)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Event handler updated successfully",
    }
    jsonResponse, _ := json.Marshal(response)
    return C.CString(string(jsonResponse))
}

//export DeleteEvent
func DeleteEvent(client unsafe.Pointer, name *C.char) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goName := C.GoString(name)
    err := apiClient.EventResourceApi.UnregisterEventHandler(goName)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Event handler unregistered successfully",
    }
    jsonResponse, _ := json.Marshal(response)
    return C.CString(string(jsonResponse))
}

//export StartWorkflow
func StartWorkflow(client unsafe.Pointer, name *C.char, version C.int, correlationId *C.char) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goName := C.GoString(name)
    goVersion := int(version)
    goCorrelationId := C.GoString(correlationId)
    
    startRequest := &model.StartWorkflowRequest{
        Name:          goName,
        Version:       goVersion,
        CorrelationId: goCorrelationId,
    }
    
    workflowId, err := apiClient.WorkflowResourceApi.StartWorkflow(startRequest)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    response := map[string]interface{}{
        "workflowId": workflowId,
        "success":    true,
    }
    jsonResponse, _ := json.Marshal(response)
    return C.CString(string(jsonResponse))
}

//export GetWorkflow
func GetWorkflow(client unsafe.Pointer, workflowId *C.char) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goWorkflowId := C.GoString(workflowId)
    workflow, err := apiClient.WorkflowResourceApi.GetExecutionStatus(goWorkflowId)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    jsonResponse, _ := json.Marshal(workflow)
    return C.CString(string(jsonResponse))
}

//export GetWorkflows
func GetWorkflows(client unsafe.Pointer) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    workflows, err := apiClient.WorkflowResourceApi.GetRunningWorkflow("", "", 100, 0)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    jsonResponse, _ := json.Marshal(workflows)
    return C.CString(string(jsonResponse))
}

//export TerminateWorkflow
func TerminateWorkflow(client unsafe.Pointer, workflowId *C.char, reason *C.char) *C.char {
    apiClient, exists := clients[client]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goWorkflowId := C.GoString(workflowId)
    goReason := C.GoString(reason)
    
    err := apiClient.WorkflowResourceApi.Terminate(goWorkflowId, goReason)
    if err != nil {
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Workflow terminated successfully",
    }
    jsonResponse, _ := json.Marshal(response)
    return C.CString(string(jsonResponse))
}

//export FreeString
func FreeString(ptr *C.char) {
    C.free(unsafe.Pointer(ptr))
}

func main() {
    // This is required for CGO but not used
    fmt.Println("Go shared library loaded")
} 