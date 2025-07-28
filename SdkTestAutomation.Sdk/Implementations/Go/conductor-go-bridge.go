package main

/*
#cgo CFLAGS: -I${SRCDIR}
#cgo LDFLAGS: -L${SRCDIR} -lstdc++
#include <stdlib.h>
#include <stdint.h>
*/
import "C"
import (
    "context"
    "encoding/json"
    "fmt"
    "log"
    "os"
    "unsafe"
    
    "github.com/conductor-sdk/conductor-go/sdk/client"
    "github.com/conductor-sdk/conductor-go/sdk/model"
)

// Global client storage with ID-based lookup
var clients = make(map[int]*client.APIClient)
var nextClientID = 1

//export CreateConductorClient
func CreateConductorClient(serverUrl *C.char) C.int {
    goServerUrl := C.GoString(serverUrl)
    log.Printf("[GO] CreateConductorClient called with server URL: %s", goServerUrl)
    
    // Set environment variable for the client
    os.Setenv("CONDUCTOR_SERVER_URL", goServerUrl)
    log.Printf("[GO] Set CONDUCTOR_SERVER_URL environment variable")
    
    // Create API client
    log.Printf("[GO] Creating API client...")
    apiClient := client.NewAPIClientFromEnv()
    log.Printf("[GO] API client created successfully")
    
    // Store API client reference with ID
    clientID := nextClientID
    nextClientID++
    clients[clientID] = apiClient
    log.Printf("[GO] Stored client with ID: %d", clientID)
    
    log.Printf("[GO] CreateConductorClient completed successfully, returning client ID: %d", clientID)
    return C.int(clientID)
}

//export DestroyConductorClient
func DestroyConductorClient(clientID C.int) {
    log.Printf("[GO] DestroyConductorClient called with client ID: %d", clientID)
    if _, exists := clients[int(clientID)]; exists {
        // Clean up client
        delete(clients, int(clientID))
        log.Printf("[GO] Successfully destroyed client with ID: %d", clientID)
        // Note: Go garbage collector will handle the rest 
    } else {
        log.Printf("[GO] WARNING: Client with ID %d not found for destruction", clientID)
    }
}

//export AddEventHandler
func AddEventHandler(clientID C.int, name *C.char, eventType *C.char, active C.int) *C.char {
    log.Printf("[GO] AddEventHandler called with client ID: %d, name: %s, eventType: %s, active: %d", 
        clientID, C.GoString(name), C.GoString(eventType), active)
    
    apiClient, exists := clients[int(clientID)]
    if !exists {
        log.Printf("[GO] ERROR: Client with ID %d not found", clientID)
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    log.Printf("[GO] Found client with ID: %d", clientID)
    
    goName := C.GoString(name)
    goEventType := C.GoString(eventType)
    goActive := active != 0
    
    log.Printf("[GO] Creating event handler with name: %s, eventType: %s, active: %v", 
        goName, goEventType, goActive)
    
    // Create a default action to satisfy Conductor's validation requirements
    defaultAction := model.Action{
        Action: "start_workflow",
        StartWorkflow: &model.StartWorkflow{
            Name:    "test_workflow",
            Version: 1,
        },
    }
    
    eventHandler := &model.EventHandler{
        Name:   goName,
        Event:  goEventType,
        Active: goActive,
        Actions: []model.Action{defaultAction},
    }
    
    log.Printf("[GO] Creating event handler client...")
    eventClient := client.NewEventHandlerClient(apiClient)
    log.Printf("[GO] Calling AddEventHandler on conductor server...")
    
    _, err := eventClient.AddEventHandler(context.Background(), *eventHandler)
    if err != nil {
        log.Printf("[GO] ERROR: AddEventHandler failed: %v", err)
        response := map[string]interface{}{
            "success": false,
            "error":   err.Error(),
        }
        jsonResponse, _ := json.Marshal(response)
        return C.CString(string(jsonResponse))
    }
    
    log.Printf("[GO] AddEventHandler completed successfully")
    response := map[string]interface{}{
        "success": true,
        "message": "Event handler registered successfully",
    }
    jsonResponse, _ := json.Marshal(response)
    return C.CString(string(jsonResponse))
}

//export GetEvents
func GetEvents(clientID C.int) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    eventClient := client.NewEventHandlerClient(apiClient)
    events, _, err := eventClient.GetEventHandlers(context.Background())
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
func GetEventByName(clientID C.int, eventName *C.char) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goEventName := C.GoString(eventName)
    eventClient := client.NewEventHandlerClient(apiClient)
    events, _, err := eventClient.GetEventHandlersForEvent(context.Background(), goEventName, nil)
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
func UpdateEvent(clientID C.int, name *C.char, eventType *C.char, active C.int) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goName := C.GoString(name)
    goEventType := C.GoString(eventType)
    goActive := active != 0
    
    // Create a default action to satisfy Conductor's validation requirements
    defaultAction := model.Action{
        Action: "start_workflow",
        StartWorkflow: &model.StartWorkflow{
            Name:    "test_workflow",
            Version: 1,
        },
    }
    
    eventHandler := &model.EventHandler{
        Name:   goName,
        Event:  goEventType,
        Active: goActive,
        Actions: []model.Action{defaultAction},
    }
    
    eventClient := client.NewEventHandlerClient(apiClient)
    _, err := eventClient.UpdateEventHandler(context.Background(), *eventHandler)
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
func DeleteEvent(clientID C.int, name *C.char) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goName := C.GoString(name)
    eventClient := client.NewEventHandlerClient(apiClient)
    _, err := eventClient.RemoveEventHandler(context.Background(), goName)
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
func StartWorkflow(clientID C.int, name *C.char, version C.int, correlationId *C.char) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goName := C.GoString(name)
    goVersion := int(version)
    goCorrelationId := C.GoString(correlationId)
    
    startRequest := map[string]interface{}{
        "name":          goName,
        "version":       goVersion,
        "correlationId": goCorrelationId,
    }
    
    workflowClient := client.NewWorkflowClient(apiClient)
    workflowId, _, err := workflowClient.StartWorkflow(context.Background(), startRequest, goName, nil)
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
func GetWorkflow(clientID C.int, workflowId *C.char) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goWorkflowId := C.GoString(workflowId)
    workflowClient := client.NewWorkflowClient(apiClient)
    workflow, _, err := workflowClient.GetExecutionStatus(context.Background(), goWorkflowId, nil)
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
func GetWorkflows(clientID C.int) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    workflowClient := client.NewWorkflowClient(apiClient)
    workflows, _, err := workflowClient.GetRunningWorkflow(context.Background(), "", nil)
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
func TerminateWorkflow(clientID C.int, workflowId *C.char, reason *C.char) *C.char {
    apiClient, exists := clients[int(clientID)]
    if !exists {
        return C.CString(`{"success":false,"error":"Client not found"}`)
    }
    
    goWorkflowId := C.GoString(workflowId)
    _ = C.GoString(reason) // Reason parameter is available but not used in current API
    
    workflowClient := client.NewWorkflowClient(apiClient)
    _, err := workflowClient.Terminate(context.Background(), goWorkflowId, nil)
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