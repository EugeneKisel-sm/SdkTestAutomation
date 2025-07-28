package main

import (
    "encoding/json"
    "log"
    "net/http"
    "os"
    
    "github.com/conductor-sdk/conductor-go/sdk/client"
    "github.com/conductor-sdk/conductor-go/sdk/model"
    "github.com/gorilla/mux"
)

var apiClient *client.APIClient

func main() {
    // Initialize API client
    apiClient = client.NewAPIClientFromEnv()
    
    // Create router
    r := mux.NewRouter()
    
    // Health check
    r.HandleFunc("/health", healthHandler).Methods("GET")
    
    // Event operations
    r.HandleFunc("/events/add", addEventHandler).Methods("POST")
    r.HandleFunc("/events/get", getEventsHandler).Methods("POST")
    r.HandleFunc("/events/getByName", getEventByNameHandler).Methods("POST")
    r.HandleFunc("/events/update", updateEventHandler).Methods("POST")
    r.HandleFunc("/events/delete", deleteEventHandler).Methods("POST")
    
    // Workflow operations
    r.HandleFunc("/workflows/get", getWorkflowHandler).Methods("POST")
    r.HandleFunc("/workflows/getAll", getWorkflowsHandler).Methods("POST")
    r.HandleFunc("/workflows/start", startWorkflowHandler).Methods("POST")
    r.HandleFunc("/workflows/terminate", terminateWorkflowHandler).Methods("POST")
    
    // Get port from environment or use default
    port := os.Getenv("GO_API_SERVER_PORT")
    if port == "" {
        port = "8081"
    }
    
    // Start server
    log.Printf("Starting Go API server on :%s", port)
    log.Fatal(http.ListenAndServe(":"+port, r))
}

func healthHandler(w http.ResponseWriter, r *http.Request) {
    w.WriteHeader(http.StatusOK)
    json.NewEncoder(w).Encode(map[string]string{"status": "ok"})
}

func addEventHandler(w http.ResponseWriter, r *http.Request) {
    var request struct {
        Name     string `json:"name"`
        Event    string `json:"event"`
        Active   bool   `json:"active"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
        http.Error(w, err.Error(), http.StatusBadRequest)
        return
    }
    
    eventHandler := &model.EventHandler{
        Name:   request.Name,
        Event:  request.Event,
        Active: request.Active,
        Actions: []model.EventAction{},
    }
    
    err := apiClient.EventResourceApi.RegisterEventHandler(eventHandler)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Event handler registered successfully",
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(response)
}

func getEventsHandler(w http.ResponseWriter, r *http.Request) {
    events, err := apiClient.EventResourceApi.GetEventHandlers("", false)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(events)
}

func getEventByNameHandler(w http.ResponseWriter, r *http.Request) {
    var request struct {
        EventName string `json:"eventName"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
        http.Error(w, err.Error(), http.StatusBadRequest)
        return
    }
    
    events, err := apiClient.EventResourceApi.GetEventHandlers(request.EventName, false)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(events)
}

func updateEventHandler(w http.ResponseWriter, r *http.Request) {
    var request struct {
        Name     string `json:"name"`
        Event    string `json:"event"`
        Active   bool   `json:"active"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
        http.Error(w, err.Error(), http.StatusBadRequest)
        return
    }
    
    eventHandler := &model.EventHandler{
        Name:   request.Name,
        Event:  request.Event,
        Active: request.Active,
        Actions: []model.EventAction{},
    }
    
    err := apiClient.EventResourceApi.UpdateEventHandler(eventHandler)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Event handler updated successfully",
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(response)
}

func deleteEventHandler(w http.ResponseWriter, r *http.Request) {
    var request struct {
        Name string `json:"name"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
        http.Error(w, err.Error(), http.StatusBadRequest)
        return
    }
    
    err := apiClient.EventResourceApi.UnregisterEventHandler(request.Name)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Event handler unregistered successfully",
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(response)
}

func getWorkflowHandler(w http.ResponseWriter, r *http.Request) {
    var request struct {
        WorkflowId string `json:"workflowId"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
        http.Error(w, err.Error(), http.StatusBadRequest)
        return
    }
    
    workflow, err := apiClient.WorkflowResourceApi.GetExecutionStatus(request.WorkflowId)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(workflow)
}

func getWorkflowsHandler(w http.ResponseWriter, r *http.Request) {
    workflows, err := apiClient.WorkflowResourceApi.GetRunningWorkflow("", "", 100, 0)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(workflows)
}

func startWorkflowHandler(w http.ResponseWriter, r *http.Request) {
    var request struct {
        Name          string `json:"name"`
        Version       int    `json:"version"`
        CorrelationId string `json:"correlationId"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
        http.Error(w, err.Error(), http.StatusBadRequest)
        return
    }
    
    startRequest := &model.StartWorkflowRequest{
        Name:          request.Name,
        Version:       request.Version,
        CorrelationId: request.CorrelationId,
    }
    
    workflowId, err := apiClient.WorkflowResourceApi.StartWorkflow(startRequest)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    response := map[string]interface{}{
        "workflowId": workflowId,
        "success":    true,
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(response)
}

func terminateWorkflowHandler(w http.ResponseWriter, r *http.Request) {
    var request struct {
        WorkflowId string `json:"workflowId"`
        Reason     string `json:"reason"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
        http.Error(w, err.Error(), http.StatusBadRequest)
        return
    }
    
    err := apiClient.WorkflowResourceApi.Terminate(request.WorkflowId, request.Reason)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    response := map[string]interface{}{
        "success": true,
        "message": "Workflow terminated successfully",
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(response)
} 