package com.sdktestautomation.utils;

import com.netflix.conductor.client.http.ConductorClient;
import com.sdktestautomation.models.SdkResponse;

public class OperationUtils {
    
    private static final String DEFAULT_SERVER_URL = "http://localhost:8080/api";
    
    public static SdkResponse executeWithErrorHandling(OperationSupplier operation) {
        try {
            return operation.execute();
        } catch (Exception e) {
            return SdkResponse.createError(500, e.getMessage());
        }
    }
    
    public static ConductorClient createSdkConfiguration() {
        String serverUrl = System.getenv("CONDUCTOR_SERVER_URL");
        if (serverUrl == null) {
            serverUrl = DEFAULT_SERVER_URL;
        }
        return new ConductorClient.Builder().basePath(serverUrl).build();
    }
    
    @FunctionalInterface
    public interface OperationSupplier {
        SdkResponse execute() throws Exception;
    }
} 