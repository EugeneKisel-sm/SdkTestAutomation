package com.sdktestautomation.operations.orkes;

import com.netflix.conductor.client.http.ConductorClient;
import io.orkes.conductor.client.http.TokenResource;
import io.orkes.conductor.client.model.GenerateTokenRequest;

import com.sdktestautomation.models.SdkResponse;
import com.sdktestautomation.utils.OperationUtils;

import java.util.Map;

public class TokenOperations {
    
    public static SdkResponse execute(String operation, Map<String, Object> parameters) {
        return OperationUtils.executeWithErrorHandling(() -> {
            ConductorClient client = OperationUtils.createSdkConfiguration();
            TokenResource tokenApi = new TokenResource(client);
            
            return switch (operation) {
                case "generate-token" -> generateToken(parameters, tokenApi);
                case "get-user-info" -> getUserInfo(tokenApi);
                default -> throw new IllegalArgumentException("Unknown token operation: " + operation);
            };
        });
    }
    
    private static SdkResponse generateToken(Map<String, Object> parameters, TokenResource tokenApi) throws Exception {
        String keyId = (String) parameters.get("keyId");
        String keySecret = (String) parameters.get("keySecret");

        GenerateTokenRequest request = new GenerateTokenRequest();
        request.setKeyId(keyId);
        request.setKeySecret(keySecret);

        var token = tokenApi.generate(request);
        return SdkResponse.createSuccess(token);
    }
    
    private static SdkResponse getUserInfo(TokenResource tokenApi) throws Exception {
        var token = tokenApi.getUserInfo();
        return SdkResponse.createSuccess(token);
    }
} 