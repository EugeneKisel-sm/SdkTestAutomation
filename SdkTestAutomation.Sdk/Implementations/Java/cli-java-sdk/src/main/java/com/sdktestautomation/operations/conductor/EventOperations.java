package com.sdktestautomation.operations.conductor;

import com.netflix.conductor.client.http.ConductorClient;
import com.netflix.conductor.client.http.EventClient;
import com.netflix.conductor.common.metadata.events.EventHandler;
import com.sdktestautomation.models.SdkResponse;
import com.sdktestautomation.utils.OperationUtils;

import java.util.List;
import java.util.Map;

public class EventOperations {
    
    public static SdkResponse execute(String operation, Map<String, Object> parameters) {
        return OperationUtils.executeWithErrorHandling(() -> {
            ConductorClient client = OperationUtils.createSdkConfiguration();
            EventClient eventApi = new EventClient(client);
            
            return switch (operation) {
                case "add-event" -> addEvent(parameters, eventApi);
                case "get-event" -> getEvent(eventApi);
                case "get-event-by-name" -> getEventByName(parameters, eventApi);
                case "update-event" -> updateEvent(parameters, eventApi);
                case "delete-event" -> deleteEvent(parameters, eventApi);
                default -> throw new IllegalArgumentException("Unknown event operation: " + operation);
            };
        });
    }
    
    private static SdkResponse addEvent(Map<String, Object> parameters, EventClient eventApi) throws Exception {
        EventHandler eventHandler = new EventHandler();
        eventHandler.setName((String) parameters.get("name"));
        eventHandler.setEvent((String) parameters.get("event"));
        eventHandler.setActive((Boolean) parameters.get("active"));
        eventHandler.setActions(List.of());
        
        eventApi.registerEventHandler(eventHandler);
        return SdkResponse.createSuccess();
    }
    
    private static SdkResponse getEvent(EventClient eventApi) throws Exception {
        List<EventHandler> events = eventApi.getEventHandlers("*", false);
        return SdkResponse.createSuccess(events);
    }
    
    private static SdkResponse getEventByName(Map<String, Object> parameters, EventClient eventApi) throws Exception {
        String eventName = (String) parameters.get("event");
        Boolean activeOnly = (Boolean) parameters.get("activeOnly");
        
        List<EventHandler> events = eventApi.getEventHandlers(eventName, activeOnly);
        return SdkResponse.createSuccess(events);
    }
    
    private static SdkResponse updateEvent(Map<String, Object> parameters, EventClient eventApi) throws Exception {
        EventHandler eventHandler = new EventHandler();
        eventHandler.setName((String) parameters.get("name"));
        eventHandler.setEvent((String) parameters.get("event"));
        eventHandler.setActive((Boolean) parameters.get("active"));
        eventHandler.setActions(List.of());
        
        eventApi.updateEventHandler(eventHandler);
        return SdkResponse.createSuccess();
    }
    
    private static SdkResponse deleteEvent(Map<String, Object> parameters, EventClient eventApi) throws Exception {
        String eventName = (String) parameters.get("name");
        eventApi.unregisterEventHandler(eventName);
        return SdkResponse.createSuccess();
    }
} 