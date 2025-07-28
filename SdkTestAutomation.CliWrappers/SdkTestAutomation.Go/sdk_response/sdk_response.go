package sdk_response

import "encoding/json"

// SdkResponse represents a standardized response from SDK operations
type SdkResponse struct {
	Success      bool        `json:"success"`
	Data         interface{} `json:"data,omitempty"`
	ErrorMessage *string     `json:"errorMessage,omitempty"`
}

// CreateSuccess creates a successful response with optional data
func CreateSuccess(data ...interface{}) *SdkResponse {
	response := &SdkResponse{
		Success: true,
	}
	
	if len(data) > 0 {
		response.Data = data[0]
	}
	
	return response
}

// CreateError creates an error response with the given message
func CreateError(statusCode int, message string) *SdkResponse {
	return &SdkResponse{
		Success:      false,
		ErrorMessage: &message,
	}
}

// ToJSON converts the response to JSON string
func (r *SdkResponse) ToJSON() (string, error) {
	bytes, err := json.Marshal(r)
	if err != nil {
		return "", err
	}
	return string(bytes), nil
} 