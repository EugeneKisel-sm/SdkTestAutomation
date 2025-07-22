package com.conductor.sdkwrapper;

import com.fasterxml.jackson.databind.ObjectMapper;
import picocli.CommandLine;
import com.netflix.conductor.client.http.ConductorClient;
import com.netflix.conductor.client.http.EventClient;
import com.netflix.conductor.client.http.WorkflowClient;
import com.netflix.conductor.common.metadata.events.EventHandler;
import com.netflix.conductor.common.run.Workflow;

import java.util.List;
import java.util.Map;

@CommandLine.Command(name = "java-sdk-wrapper", mixinStandardHelpOptions = true)
public class Main implements Runnable {
    
    @CommandLine.Option(names = "--operation", required = true, description = "SDK operation")
    private String operation;
    
    @CommandLine.Option(names = "--parameters", required = true, description = "JSON parameters")
    private String parameters;
    
    @CommandLine.Option(names = "--resource", required = true, description = "Resource type")
    private String resource;
    
    public static void main(String[] args) {
        CommandLine.run(new Main(), args);
    }
    
    @Override
    public void run() {
        try {
            ObjectMapper mapper = new ObjectMapper();
            String serverUrl = System.getenv("CONDUCTOR_SERVER_URL");
            if (serverUrl == null) {
                serverUrl = "http://localhost:8080/api";
            }
            
            ConductorClient client = new ConductorClient.Builder().basePath(serverUrl).build();
            EventClient eventApi = new EventClient(client);
            WorkflowClient workflowApi = new WorkflowClient(client);
            
            SdkResponse result = executeSdkOperation(operation, parameters, resource, eventApi, workflowApi);
            System.out.println(mapper.writeValueAsString(result));
        } catch (Exception e) {
            try {
                SdkResponse error = new SdkResponse(false, e.getMessage(), 500);
                ObjectMapper mapper = new ObjectMapper();
                System.out.println(mapper.writeValueAsString(error));
            } catch (Exception ex) {
                System.err.println("Error serializing error response: " + ex.getMessage());
            }
            System.exit(1);
        }
    }
    
    private static SdkResponse executeSdkOperation(String operation, String parameters, String resource, 
                                                  EventClient eventApi, WorkflowClient workflowApi) throws Exception {
        ObjectMapper mapper = new ObjectMapper();
        Map<String, Object> params = mapper.readValue(parameters, Map.class);
        
        return switch (operation) {
            case "add-event" -> addEvent(params, eventApi);
            case "get-event" -> getEvent(params, eventApi);
            case "get-event-by-name" -> getEventByName(params, eventApi);
            case "update-event" -> updateEvent(params, eventApi);
            case "delete-event" -> deleteEvent(params, eventApi);
            case "get-workflow" -> getWorkflow(params, workflowApi);
            default -> throw new IllegalArgumentException("Unknown operation: " + operation);
        };
    }
    
    private static SdkResponse addEvent(Map<String, Object> parameters, EventClient eventApi) {
        try {
            EventHandler eventHandler = new EventHandler();
            eventHandler.setName((String) parameters.get("name"));
            eventHandler.setEvent((String) parameters.get("event"));
            eventHandler.setActive((Boolean) parameters.get("active"));
            
            // For now, create a simple action list to satisfy the API
            // The actual action structure will be determined by the SDK's requirements
            eventHandler.setActions(List.of());
            
            eventApi.registerEventHandler(eventHandler);
            
            return new SdkResponse(true, null, 200);
        } catch (Exception e) {
            return new SdkResponse(false, e.getMessage(), 500);
        }
    }
    
    private static SdkResponse getEvent(Map<String, Object> parameters, EventClient eventApi) {
        try {
            List<EventHandler> events = eventApi.getEventHandlers("", false);
            ObjectMapper mapper = new ObjectMapper();
            String content = mapper.writeValueAsString(events);
            
            return new SdkResponse(true, content, 200, events);
        } catch (Exception e) {
            return new SdkResponse(false, e.getMessage(), 500);
        }
    }
    
    private static SdkResponse getEventByName(Map<String, Object> parameters, EventClient eventApi) {
        try {
            String eventName = (String) parameters.get("event");
            Boolean activeOnly = (Boolean) parameters.get("activeOnly");
            
            List<EventHandler> events = eventApi.getEventHandlers(eventName, activeOnly);
            ObjectMapper mapper = new ObjectMapper();
            String content = mapper.writeValueAsString(events);
            
            return new SdkResponse(true, content, 200, events);
        } catch (Exception e) {
            return new SdkResponse(false, e.getMessage(), 500);
        }
    }
    
    private static SdkResponse updateEvent(Map<String, Object> parameters, EventClient eventApi) {
        try {
            EventHandler eventHandler = new EventHandler();
            eventHandler.setName((String) parameters.get("name"));
            eventHandler.setEvent((String) parameters.get("event"));
            eventHandler.setActive((Boolean) parameters.get("active"));
            
            // For now, create a simple action list to satisfy the API
            eventHandler.setActions(List.of());
            
            eventApi.updateEventHandler(eventHandler);
            
            return new SdkResponse(true, null, 200);
        } catch (Exception e) {
            return new SdkResponse(false, e.getMessage(), 500);
        }
    }
    
    private static SdkResponse deleteEvent(Map<String, Object> parameters, EventClient eventApi) {
        try {
            String eventName = (String) parameters.get("name");
            
            eventApi.unregisterEventHandler(eventName);
            
            return new SdkResponse(true, null, 200);
        } catch (Exception e) {
            return new SdkResponse(false, e.getMessage(), 500);
        }
    }
    
    private static SdkResponse getWorkflow(Map<String, Object> parameters, WorkflowClient workflowApi) {
        try {
            String workflowId = (String) parameters.get("workflowId");
            
            Workflow workflow = workflowApi.getWorkflow(workflowId, false);
            ObjectMapper mapper = new ObjectMapper();
            String content = mapper.writeValueAsString(workflow);
            
            return new SdkResponse(true, content, 200, workflow);
        } catch (Exception e) {
            return new SdkResponse(false, e.getMessage(), 500);
        }
    }
} 