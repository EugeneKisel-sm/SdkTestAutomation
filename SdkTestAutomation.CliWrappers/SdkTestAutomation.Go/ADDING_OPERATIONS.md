# Adding Operations to Go CLI Wrapper

This guide explains how to add new operations to the Go CLI wrapper for testing the Conductor Go SDK.

## 📁 Structure

```
SdkTestAutomation.Go/
├── main.go                    # CLI entry point
├── go.mod                     # Go module dependencies
├── sdk_response/
│   └── sdk_response.go        # Response structure
├── operation_utils/
│   └── operation_utils.go     # Common utilities
└── operations/
    ├── event_operations.go    # Event resource operations
    └── workflow_operations.go # Workflow resource operations
```

## 🔧 Adding a New Operation

### Step 1: Choose the Resource Type

Operations are organized by resource type:
- **Event operations**: `operations/event_operations.go`
- **Workflow operations**: `operations/workflow_operations.go`

### Step 2: Add the Operation Function

Add your operation function to the appropriate file:

```go
func yourOperation(parameters map[string]interface{}, apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
    // Extract parameters
    param1 := getStringParameter(parameters, "param1")
    param2 := getBoolParameter(parameters, "param2")
    
    // Call the SDK
    result, err := apiClient.YourResourceApi.YourMethod(param1, param2)
    if err != nil {
        return nil, fmt.Errorf("failed to execute operation: %w", err)
    }
    
    // Return success response
    return sdk_response.CreateSuccess(result), nil
}
```

### Step 3: Add to the Switch Statement

Add your operation to the switch statement in the main execution function:

```go
func ExecuteEventOperation(operation string, parameters map[string]interface{}) (*sdk_response.SdkResponse, error) {
    apiClient := operation_utils.CreateApiClient()
    
    switch operation {
    case "add-event":
        return addEvent(parameters, apiClient)
    case "your-operation":  // Add this line
        return yourOperation(parameters, apiClient)  // Add this line
    default:
        return nil, fmt.Errorf("unknown event operation: %s", operation)
    }
}
```

### Step 4: Add Parameter Helpers (if needed)

If you need new parameter types, add helper functions:

```go
func getIntParameter(parameters map[string]interface{}, key string) int {
    if value, exists := parameters[key]; exists {
        switch v := value.(type) {
        case int:
            return v
        case float64:
            return int(v)
        case string:
            if i, err := strconv.Atoi(v); err == nil {
                return i
            }
        }
    }
    return 0
}
```

## 📝 Example: Adding a New Event Operation

Let's add a `get-event-by-id` operation:

### 1. Add the Operation Function

```go
func getEventById(parameters map[string]interface{}, apiClient *client.APIClient) (*sdk_response.SdkResponse, error) {
    eventId := getStringParameter(parameters, "eventId")
    
    event, err := apiClient.EventResourceApi.GetEventHandlerById(eventId)
    if err != nil {
        return nil, fmt.Errorf("failed to get event handler by ID: %w", err)
    }
    
    return sdk_response.CreateSuccess(event), nil
}
```

### 2. Add to Switch Statement

```go
case "get-event-by-id":
    return getEventById(parameters, apiClient)
```

### 3. Test the Operation

```bash
./go-sdk-wrapper \
  --operation get-event-by-id \
  --parameters '{"eventId":"test-event-id"}' \
  --resource event
```

## 🔍 Parameter Extraction Helpers

The wrapper provides these parameter extraction helpers:

- `getStringParameter(parameters, key)` - Extract string parameters
- `getBoolParameter(parameters, key)` - Extract boolean parameters

## 📋 Response Format

All operations must return a `*sdk_response.SdkResponse`:

```go
// Success response with data
return sdk_response.CreateSuccess(result), nil

// Success response without data
return sdk_response.CreateSuccess(), nil

// Error response
return nil, fmt.Errorf("operation failed: %w", err)
```

## 🧪 Testing

After adding an operation:

1. **Build the wrapper**:
   ```bash
   go build -o go-sdk-wrapper
   ```

2. **Test the operation**:
   ```bash
   ./go-sdk-wrapper --operation your-operation --parameters '{"key":"value"}' --resource event
   ```

3. **Verify the response format**:
   ```json
   {
     "success": true,
     "data": { /* operation result */ }
   }
   ```

## 📚 Related Documentation

- **[Universal Operations Guide](../../ADDING_OPERATIONS_GUIDE.md)** - Cross-language patterns and best practices
- **[Go SDK Documentation](https://github.com/conductor-oss/go-sdk)** - Official Go SDK documentation
- **[CLI Wrapper README](README.md)** - Go wrapper usage guide 