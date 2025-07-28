package operations

import (
	"context"
	"fmt"
	"strconv"

	"github.com/antihax/optional"
	"github.com/conductor-sdk/conductor-go/sdk/client"
	"github.com/conductor-sdk/conductor-go/sdk/model"
	"github.com/conductor-oss/sdk-test-automation-go/operation_utils"
	"github.com/conductor-oss/sdk-test-automation-go/sdk_response"
)

// ExecuteEventOperation executes event-related operations
func ExecuteEventOperation(operation string, parameters map[string]interface{}) (*sdk_response.SdkResponse, error) {
	apiClient := operation_utils.CreateApiClient()
	
	switch operation {
	case "add-event":
		return addEvent(parameters, apiClient)
	case "get-event":
		return getEvent(apiClient)
	case "get-event-by-name":
		return getEventByName(parameters, apiClient)
	case "update-event":
		return updateEvent(parameters, apiClient)
	case "delete-event":
		return deleteEvent(parameters, apiClient)
	default:
		return nil, fmt.Errorf("unknown event operation: %s", operation)
	}
}

func addEvent(parameters map[string]interface{}, apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
	eventHandler := createEventHandler(parameters)
	eventClient := client.NewEventHandlerClient(apiClient)
	
	_, err := eventClient.AddEventHandler(context.Background(), *eventHandler)
	if err != nil {
		return nil, fmt.Errorf("failed to add event handler: %w", err)
	}
	
	return sdk_response.CreateSuccess(), nil
}

func getEvent(apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
	eventClient := client.NewEventHandlerClient(apiClient)
	
	events, _, err := eventClient.GetEventHandlers(context.Background())
	if err != nil {
		return nil, fmt.Errorf("failed to get event handlers: %w", err)
	}
	
	return sdk_response.CreateSuccess(events), nil
}

func getEventByName(parameters map[string]interface{}, apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
	eventName := getStringParameter(parameters, "event")
	activeOnly := getBoolParameter(parameters, "activeOnly")
	eventClient := client.NewEventHandlerClient(apiClient)
	
	opts := &client.EventResourceApiGetEventHandlersForEventOpts{
		ActiveOnly: optional.NewBool(activeOnly),
	}
	
	events, _, err := eventClient.GetEventHandlersForEvent(context.Background(), eventName, opts)
	if err != nil {
		return nil, fmt.Errorf("failed to get event handlers by name: %w", err)
	}
	
	return sdk_response.CreateSuccess(events), nil
}

func updateEvent(parameters map[string]interface{}, apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
	eventHandler := createEventHandler(parameters)
	eventClient := client.NewEventHandlerClient(apiClient)
	
	_, err := eventClient.UpdateEventHandler(context.Background(), *eventHandler)
	if err != nil {
		return nil, fmt.Errorf("failed to update event handler: %w", err)
	}
	
	return sdk_response.CreateSuccess(), nil
}

func deleteEvent(parameters map[string]interface{}, apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
	eventName := getStringParameter(parameters, "name")
	eventClient := client.NewEventHandlerClient(apiClient)
	
	_, err := eventClient.RemoveEventHandler(context.Background(), eventName)
	if err != nil {
		return nil, fmt.Errorf("failed to delete event handler: %w", err)
	}
	
	return sdk_response.CreateSuccess(), nil
}

func createEventHandler(parameters map[string]interface{}) *model.EventHandler {
	return &model.EventHandler{
		Name:    getStringParameter(parameters, "name"),
		Event:   getStringParameter(parameters, "event"),
		Active:  getBoolParameter(parameters, "active"),
		Actions: []model.Action{},
	}
}

// Helper functions for parameter extraction
func getStringParameter(parameters map[string]interface{}, key string) string {
	if value, exists := parameters[key]; exists {
		if str, ok := value.(string); ok {
			return str
		}
	}
	return ""
}

func getBoolParameter(parameters map[string]interface{}, key string) bool {
	if value, exists := parameters[key]; exists {
		switch v := value.(type) {
		case bool:
			return v
		case string:
			if b, err := strconv.ParseBool(v); err == nil {
				return b
			}
		}
	}
	return false
} 