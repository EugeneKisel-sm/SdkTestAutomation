package com.sdktestautomation;

import picocli.CommandLine;
import com.sdktestautomation.models.SdkResponse;
import com.sdktestautomation.operations.orkes.TokenOperations;

import java.util.Map;

@CommandLine.Command(name = "orkes-cli", mixinStandardHelpOptions = true)
public class OrkesCli extends BaseCli {
    
    public static void main(String[] args) {
        new CommandLine(new OrkesCli()).execute(args);
    }
    
    @Override
    protected SdkResponse executeOperation(String resource, String operation, Map<String, Object> parameters) {
        return switch (resource.toLowerCase()) {
            case "token" -> TokenOperations.execute(operation, parameters);
            default -> throw new IllegalArgumentException("Unknown resource: " + resource);
        };
    }
} 