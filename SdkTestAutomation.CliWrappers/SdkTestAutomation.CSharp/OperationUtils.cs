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
    
    public static Configuration CreateSdkConfiguration()
    {
        var serverUrl = Environment.GetEnvironmentVariable("CONDUCTOR_SERVER_URL") ?? "http://localhost:8080/api";
        return new Configuration { BasePath = serverUrl };
    }
} 