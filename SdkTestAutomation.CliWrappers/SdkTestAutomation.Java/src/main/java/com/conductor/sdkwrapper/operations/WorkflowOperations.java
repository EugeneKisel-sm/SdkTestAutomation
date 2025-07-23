package com.conductor.sdkwrapper.operations;

import com.netflix.conductor.client.http.ConductorClient;
import com.netflix.conductor.client.http.WorkflowClient;
import com.netflix.conductor.common.run.Workflow;
import com.conductor.sdkwrapper.OperationUtils;
import com.conductor.sdkwrapper.SdkResponse;

import java.util.Map;

public class WorkflowOperations {
    
    public static SdkResponse execute(String operation, Map<String, Object> parameters) {
        return OperationUtils.executeWithErrorHandling(() -> {
            ConductorClient client = OperationUtils.createSdkConfiguration();
            WorkflowClient workflowApi = new WorkflowClient(client);
            
            return switch (operation) {
                case "get-workflow" -> getWorkflow(parameters, workflowApi);
                default -> throw new IllegalArgumentException("Unknown workflow operation: " + operation);
            };
        });
    }
    
    private static SdkResponse getWorkflow(Map<String, Object> parameters, WorkflowClient workflowApi) throws Exception {
        String workflowId = (String) parameters.get("workflowId");
        Workflow workflow = workflowApi.getWorkflow(workflowId, false);
        return SdkResponse.createSuccess(workflow);
    }
} 