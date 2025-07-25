using System.Text.Json;
using RestSharp;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Sdk.Helpers;

public class ResponseComparer(ILogger logger)
{
    public Task<bool> CompareAsync<T>(SdkResponse<T> sdkResponse, RestResponse<T> apiResponse)
    {
        logger.Log("Comparing SDK and API responses...");
        
        var apiSuccess = apiResponse.StatusCode == System.Net.HttpStatusCode.OK;
        if (sdkResponse.StatusCode != (int)apiResponse.StatusCode || sdkResponse.Success != apiSuccess)
        {
            logger.Log($"Status mismatch: SDK={sdkResponse.StatusCode}({sdkResponse.Success}), API={(int)apiResponse.StatusCode}({apiSuccess})");
            return Task.FromResult(false);
        }
        
        if (!sdkResponse.Success && !apiSuccess)
        {
            logger.Log("Both SDK and API failed - considering equal");
            return Task.FromResult(true);
        }
        
        if (sdkResponse.Success && apiSuccess)
        {
            try
            {
                var sdkJson = JsonSerializer.Deserialize<JsonElement>(sdkResponse.Content);
                var apiJson = JsonSerializer.Deserialize<JsonElement>(apiResponse.Content ?? "{}");
                
                var isEqual = JsonElementEquals(sdkJson, apiJson);
                logger.Log($"Content comparison result: {isEqual}");
                return Task.FromResult(isEqual);
            }
            catch (Exception ex)
            {
                logger.Log($"Error during JSON comparison: {ex.Message}");
                return Task.FromResult(false);
            }
        }
        
        return Task.FromResult(false);
    }
    
    private bool JsonElementEquals(JsonElement element1, JsonElement element2)
    {
        if (element1.ValueKind != element2.ValueKind) return false;
        
        return element1.ValueKind switch
        {
            JsonValueKind.Object => CompareObject(element1, element2),
            JsonValueKind.Array => CompareArray(element1, element2),
            JsonValueKind.String => element1.GetString() == element2.GetString(),
            JsonValueKind.Number => element1.GetDecimal() == element2.GetDecimal(),
            JsonValueKind.True or JsonValueKind.False => element1.GetBoolean() == element2.GetBoolean(),
            JsonValueKind.Null => true,
            _ => false
        };
    }
    
    private bool CompareObject(JsonElement obj1, JsonElement obj2)
    {
        var properties1 = obj1.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
        var properties2 = obj2.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
        
        if (properties1.Count != properties2.Count) return false;
        
        foreach (var kvp in properties1)
        {
            if (!properties2.TryGetValue(kvp.Key, out var value2)) return false;
            if (!JsonElementEquals(kvp.Value, value2)) return false;
        }
        
        return true;
    }
    
    private bool CompareArray(JsonElement arr1, JsonElement arr2)
    {
        var elements1 = arr1.EnumerateArray().ToList();
        var elements2 = arr2.EnumerateArray().ToList();
        
        if (elements1.Count != elements2.Count) return false;

        return !elements1.Where((t, i) => !JsonElementEquals(t, elements2[i])).Any();
    }
} 