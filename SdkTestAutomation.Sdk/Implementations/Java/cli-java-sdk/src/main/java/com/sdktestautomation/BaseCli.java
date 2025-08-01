package com.sdktestautomation;

import com.fasterxml.jackson.databind.ObjectMapper;
import picocli.CommandLine;
import com.sdktestautomation.models.SdkResponse;

import java.util.Map;

public abstract class BaseCli implements Runnable {
    
    private static final ObjectMapper OBJECT_MAPPER = new ObjectMapper();
    
    @CommandLine.Option(names = "--resource", required = true, description = "Resource type")
    protected String resource;
    
    @CommandLine.Option(names = "--operation", required = true, description = "SDK operation")
    protected String operation;
    
    @CommandLine.Option(names = "--parameters", required = true, description = "JSON parameters")
    protected String parameters;
    
    @Override
    public void run() {
        try {
            @SuppressWarnings("unchecked")
            Map<String, Object> params = OBJECT_MAPPER.readValue(parameters, Map.class);
            
            SdkResponse result = executeOperation(resource, operation, params);
            System.out.println(OBJECT_MAPPER.writeValueAsString(result));
        } catch (Exception e) {
            handleError(e);
        }
    }
    
    protected abstract SdkResponse executeOperation(String resource, String operation, Map<String, Object> parameters);
    
    protected void handleError(Exception e) {
        try {
            SdkResponse error = SdkResponse.createError(500, e.getMessage());
            System.out.println(OBJECT_MAPPER.writeValueAsString(error));
        } catch (Exception ex) {
            System.err.println("Error serializing error response: " + ex.getMessage());
        }
        System.exit(1);
    }
    
    protected static ObjectMapper getObjectMapper() {
        return OBJECT_MAPPER;
    }
} 