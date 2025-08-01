package com.sdktestautomation.utils;

import com.netflix.conductor.client.http.ConductorClient;
import com.sdktestautomation.models.SdkResponse;

public class OperationUtils {
    
    private static final String DEFAULT_SERVER_URL = "http://localhost:8080/api";
    
    public static SdkResponse executeWithErrorHandling(OperationSupplier operation) {
        try {
            return operation.execute();
        } catch (Exception e) {
            System.err.println("Error in executeWithErrorHandling: " + e.getMessage());
            e.printStackTrace(System.err);
            return SdkResponse.createError(500, e.getMessage());
        }
    }
    
    public static ConductorClient createSdkConfiguration() {
        String serverUrl = System.getenv("CONDUCTOR_SERVER_URL");
        if (serverUrl == null) {
            serverUrl = DEFAULT_SERVER_URL;
        }
        
        System.err.println("Debug: Creating ConductorClient with server URL: " + serverUrl);
        
        try {
            return new ConductorClient.Builder().basePath(serverUrl).build();
        } catch (Exception e) {
            System.err.println("Error creating ConductorClient: " + e.getMessage());
            e.printStackTrace(System.err);
            throw e;
        }
    }
    
    @FunctionalInterface
    public interface OperationSupplier {
        SdkResponse execute() throws Exception;
    }
} 