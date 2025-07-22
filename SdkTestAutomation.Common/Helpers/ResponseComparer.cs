using System.Text.Json;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Common.Helpers;

public class ResponseComparer
{
    private readonly ILogger _logger;
    
    public ResponseComparer(ILogger logger)
    {
        _logger = logger;
    }
    
    public Task<bool> CompareAsync<T>(SdkResponse<T> sdkResponse, RestSharp.RestResponse<T> apiResponse)
    {
        _logger.Log("Comparing SDK and API responses...");
        
        // Compare status codes and success status
        var apiSuccess = apiResponse.StatusCode == System.Net.HttpStatusCode.OK;
        if (sdkResponse.StatusCode != (int)apiResponse.StatusCode || sdkResponse.Success != apiSuccess)
        {
            _logger.Log($"Status mismatch: SDK={sdkResponse.StatusCode}({sdkResponse.Success}), API={(int)apiResponse.StatusCode}({apiSuccess})");
            return Task.FromResult(false);
        }
        
        // Compare content if available
        if (!string.IsNullOrEmpty(sdkResponse.Content) && !string.IsNullOrEmpty(apiResponse.Content))
        {
            try
            {
                var sdkJson = JsonSerializer.Deserialize<JsonElement>(sdkResponse.Content);
                var apiJson = JsonSerializer.Deserialize<JsonElement>(apiResponse.Content);
                
                if (!JsonElementEquals(sdkJson, apiJson))
                {
                    _logger.Log("Content mismatch between SDK and API responses");
                    _logger.Log($"SDK Content: {sdkResponse.Content}");
                    _logger.Log($"API Content: {apiResponse.Content}");
                    return Task.FromResult(false);
                }
            }
            catch (JsonException ex)
            {
                _logger.Log($"Error comparing JSON content: {ex.Message}");
                return Task.FromResult(false);
            }
        }
        
        _logger.Log("SDK and API responses match");
        return Task.FromResult(true);
    }
    
    private bool JsonElementEquals(JsonElement element1, JsonElement element2)
    {
        if (element1.ValueKind != element2.ValueKind)
            return false;
            
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
    
    private bool CompareObject(JsonElement element1, JsonElement element2)
    {
        var properties1 = element1.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
        var properties2 = element2.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
        
        if (properties1.Count != properties2.Count)
            return false;
            
        return properties1.All(kvp => 
            properties2.TryGetValue(kvp.Key, out var value2) && JsonElementEquals(kvp.Value, value2));
    }
    
    private bool CompareArray(JsonElement element1, JsonElement element2)
    {
        var array1 = element1.EnumerateArray().ToArray();
        var array2 = element2.EnumerateArray().ToArray();
        
        if (array1.Length != array2.Length)
            return false;
            
        return array1.Select((item, index) => JsonElementEquals(item, array2[index])).All(x => x);
    }
} 