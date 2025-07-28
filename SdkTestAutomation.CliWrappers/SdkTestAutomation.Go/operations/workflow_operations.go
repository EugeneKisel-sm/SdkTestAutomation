package operations

import (
	"context"
	"fmt"

	"github.com/conductor-sdk/conductor-go/sdk/client"
	"github.com/conductor-oss/sdk-test-automation-go/operation_utils"
	"github.com/conductor-oss/sdk-test-automation-go/sdk_response"
)

// ExecuteWorkflowOperation executes workflow-related operations
func ExecuteWorkflowOperation(operation string, parameters map[string]interface{}) (*sdk_response.SdkResponse, error) {
	apiClient := operation_utils.CreateApiClient()
	
	switch operation {
	case "get-workflow":
		return getWorkflow(parameters, apiClient)
	default:
		return nil, fmt.Errorf("unknown workflow operation: %s", operation)
	}
}

func getWorkflow(parameters map[string]interface{}, apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
	workflowId := getStringParameter(parameters, "workflowId")
	workflowClient := client.NewWorkflowClient(apiClient)
	
	workflow, _, err := workflowClient.GetExecutionStatus(context.Background(), workflowId, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to get workflow execution status: %w", err)
	}
	
	return sdk_response.CreateSuccess(workflow), nil
} 