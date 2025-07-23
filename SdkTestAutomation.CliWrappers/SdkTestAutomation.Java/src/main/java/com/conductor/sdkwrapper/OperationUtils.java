package com.conductor.sdkwrapper;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.netflix.conductor.client.http.ConductorClient;

public class OperationUtils {
    
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
            serverUrl = "http://localhost:8080/api";
        }
        return new ConductorClient.Builder().basePath(serverUrl).build();
    }
    
    @FunctionalInterface
    public interface OperationSupplier {
        SdkResponse execute() throws Exception;
    }
} 