package main

import (
	"encoding/json"
	"fmt"
	"os"

	"github.com/spf13/cobra"
	"github.com/conductor-oss/sdk-test-automation-go/operations"
	"github.com/conductor-oss/sdk-test-automation-go/sdk_response"
)

func main() {
	var operation, parameters, resource string

	var rootCmd = &cobra.Command{
		Use:   "go-sdk-wrapper",
		Short: "Go Conductor SDK Test Wrapper",
		RunE: func(cmd *cobra.Command, args []string) error {
			var params map[string]interface{}
			if err := json.Unmarshal([]byte(parameters), &params); err != nil {
				return fmt.Errorf("failed to parse parameters: %w", err)
			}

			result, err := executeOperation(operation, params, resource)
			if err != nil {
				errorResponse := sdk_response.CreateError(500, err.Error())
				output, _ := json.Marshal(errorResponse)
				fmt.Print(string(output))
				os.Exit(1)
			}

			output, _ := json.Marshal(result)
			fmt.Print(string(output))
			return nil
		},
	}

	rootCmd.Flags().StringVarP(&operation, "operation", "o", "", "SDK operation")
	rootCmd.Flags().StringVarP(&parameters, "parameters", "p", "{}", "JSON parameters")
	rootCmd.Flags().StringVarP(&resource, "resource", "r", "", "Resource type")

	rootCmd.MarkFlagRequired("operation")
	rootCmd.MarkFlagRequired("parameters")
	rootCmd.MarkFlagRequired("resource")

	if err := rootCmd.Execute(); err != nil {
		fmt.Fprintf(os.Stderr, "Error: %v\n", err)
		os.Exit(1)
	}
}

func executeOperation(operation string, parameters map[string]interface{}, resource string) (*sdk_response.SdkResponse, error) {
	switch resource {
	case "event":
		return operations.ExecuteEventOperation(operation, parameters)
	case "workflow":
		return operations.ExecuteWorkflowOperation(operation, parameters)
	default:
		return nil, fmt.Errorf("unknown resource: %s", resource)
	}
} 