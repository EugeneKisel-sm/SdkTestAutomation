package operation_utils

import (
	"os"

	"github.com/conductor-sdk/conductor-go/sdk/client"
	"github.com/conductor-sdk/conductor-go/sdk/settings"
)

// CreateApiClient creates a Conductor API client from environment variables
func CreateApiClient() *client.APIClient {
	// Use the environment-based client creation
	return client.NewAPIClientFromEnv()
}

// CreateCustomApiClient creates a Conductor API client with custom settings
func CreateCustomApiClient() *client.APIClient {
	serverURL := getEnvOrDefault("CONDUCTOR_SERVER_URL", "http://localhost:8080/api")
	authKey := os.Getenv("CONDUCTOR_AUTH_KEY")
	authSecret := os.Getenv("CONDUCTOR_AUTH_SECRET")

	// Create HTTP settings
	httpSettings := settings.NewHttpSettings(serverURL)

	// Create authentication settings if provided
	var authSettings *settings.AuthenticationSettings
	if authKey != "" && authSecret != "" {
		authSettings = settings.NewAuthenticationSettings(authKey, authSecret)
	}

	// Create API client
	return client.NewAPIClient(authSettings, httpSettings)
}

// getEnvOrDefault returns the environment variable value or a default if not set
func getEnvOrDefault(key, defaultValue string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}
	return defaultValue
}

// ExecuteWithErrorHandling executes a function and handles errors consistently
func ExecuteWithErrorHandling[T any](fn func() (T, error)) (*T, error) {
	result, err := fn()
	if err != nil {
		return nil, err
	}
	return &result, nil
} 