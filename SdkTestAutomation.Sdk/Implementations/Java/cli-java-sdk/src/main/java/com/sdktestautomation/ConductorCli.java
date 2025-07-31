package com.sdktestautomation;

import picocli.CommandLine;
import com.sdktestautomation.models.SdkResponse;
import com.sdktestautomation.operations.conductor.EventOperations;
import com.sdktestautomation.operations.conductor.WorkflowOperations;

import java.util.Map;

@CommandLine.Command(name = "conductor-cli", mixinStandardHelpOptions = true)
public class ConductorCli extends BaseCli {
    
    public static void main(String[] args) {
        new CommandLine(new ConductorCli()).execute(args);
    }
    
    @Override
    protected SdkResponse executeOperation(String resource, String operation, Map<String, Object> parameters) {
        return switch (resource.toLowerCase()) {
            case "event" -> EventOperations.execute(operation, parameters);
            case "workflow" -> WorkflowOperations.execute(operation, parameters);
            default -> throw new IllegalArgumentException("Unknown resource: " + resource);
        };
    }
} 