package com.conductor.sdkwrapper;

import com.fasterxml.jackson.databind.ObjectMapper;
import picocli.CommandLine;
import com.conductor.sdkwrapper.operations.EventOperations;
import com.conductor.sdkwrapper.operations.WorkflowOperations;

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
            Map<String, Object> params = mapper.readValue(parameters, Map.class);
            
            SdkResponse result = executeOperation(operation, params, resource);
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
    
    private static SdkResponse executeOperation(String operation, Map<String, Object> parameters, String resource) {
        return switch (resource) {
            case "event" -> EventOperations.execute(operation, parameters);
            case "workflow" -> WorkflowOperations.execute(operation, parameters);
            default -> throw new IllegalArgumentException("Unknown resource: " + resource);
        };
    }
} 