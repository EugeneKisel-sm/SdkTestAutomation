package com.sdktestautomation;

import com.fasterxml.jackson.databind.ObjectMapper;
import picocli.CommandLine;

import com.sdktestautomation.models.SdkResponse;
import com.sdktestautomation.operations.orkes.TokenOperations;

import java.util.Map;

@CommandLine.Command(name = "orkes-cli", mixinStandardHelpOptions = true)
public class OrkesCli implements Runnable {
    
    @CommandLine.Option(names = "--resource", required = true, description = "Resource type (token)")
    private String resource;
    
    @CommandLine.Option(names = "--operation", required = true, description = "SDK operation")
    private String operation;
    
    @CommandLine.Option(names = "--parameters", required = true, description = "JSON parameters")
    private String parameters;
    
    public static void main(String[] args) {
        new CommandLine(new OrkesCli()).execute(args);
    }
    
    @Override
    public void run() {
        try {
            ObjectMapper mapper = new ObjectMapper();
            @SuppressWarnings("unchecked")
            Map<String, Object> params = mapper.readValue(parameters, Map.class);
            
            SdkResponse result = executeOperation(resource, operation, params);
            System.out.println(mapper.writeValueAsString(result));
        } catch (Exception e) {
            try {
                SdkResponse error = SdkResponse.createError(500, e.getMessage());
                ObjectMapper mapper = new ObjectMapper();
                System.out.println(mapper.writeValueAsString(error));
            } catch (Exception ex) {
                System.err.println("Error serializing error response: " + ex.getMessage());
            }
            System.exit(1);
        }
    }
    
    private static SdkResponse executeOperation(String resource, String operation, Map<String, Object> parameters) {
        return switch (resource.toLowerCase()) {
            case "token" -> TokenOperations.execute(operation, parameters);
            default -> throw new IllegalArgumentException("Unknown resource: " + resource);
        };
    }
} 