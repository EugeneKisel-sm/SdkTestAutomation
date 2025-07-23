using Conductor.Api;
using Conductor.Client;
using SdkTestAutomation.Sdk.Models;

namespace SdkTestAutomation.CSharp;

public static class OperationUtils
{
    public static SdkResponse ExecuteWithErrorHandling(Func<SdkResponse> operation)
    {
        try
        {
            return operation();
        }
        catch (ApiException ex)
        {
            return SdkResponse.CreateError(ex.ErrorCode, ex.Message);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(500, ex.Message);
        }
    }
    
    /// <summary>
    /// Creates SDK configuration with server URL from environment variable or default
    /// </summary>
    /// <returns>Conductor SDK configuration</returns>
    public static Configuration CreateSdkConfiguration()
    {
        var serverUrl = Environment.GetEnvironmentVariable("CONDUCTOR_SERVER_URL") ?? "http://localhost:8080/api";
        return new Configuration { BasePath = serverUrl };
    }
} 