using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Sdk;

public class ResponseComparer(ILogger logger)
{
    public Task<bool> CompareAsync<T>(SdkResponse<T> sdkResponse, RestSharp.RestResponse<T> apiResponse)
    {
        logger.Log("Comparing SDK and API responses...");
        
        var apiSuccess = apiResponse.StatusCode == System.Net.HttpStatusCode.OK;
        if (sdkResponse.StatusCode != (int)apiResponse.StatusCode || sdkResponse.Success != apiSuccess)
        {
            logger.Log($"Status mismatch: SDK={sdkResponse.StatusCode}({sdkResponse.Success}), API={(int)apiResponse.StatusCode}({apiSuccess}).");
            return Task.FromResult(false);
        }
        
        if (!string.IsNullOrEmpty(sdkResponse.Content) && !string.IsNullOrEmpty(apiResponse.Content))
        {
            try
            {
                var sdkJson = JToken.Parse(sdkResponse.Content);
                var apiJson = JToken.Parse(apiResponse.Content);
                
                if (!JTokenEquals(sdkJson, apiJson))
                {
                    logger.Log("Content mismatch between SDK and API responses.");
                    logger.Log($"SDK Content: {sdkResponse.Content}");
                    logger.Log($"API Content: {apiResponse.Content}");
                    return Task.FromResult(false);
                }
            }
            catch (JsonReaderException ex)
            {
                logger.Log($"Error comparing JSON content: {ex.Message}.");
                return Task.FromResult(false);
            }
        }
        
        logger.Log("SDK and API responses match.");
        return Task.FromResult(true);
    }
    
    private bool JTokenEquals(JToken token1, JToken token2)
    {
        if (token1.Type != token2.Type)
            return false;
            
        return token1.Type switch
        {
            JTokenType.Object => CompareObject(token1, token2),
            JTokenType.Array => CompareArray(token1, token2),
            JTokenType.String => token1.ToString() == token2.ToString(),
            JTokenType.Integer => token1.Value<long>() == token2.Value<long>(),
            JTokenType.Float => token1.Value<double>() == token2.Value<double>(),
            JTokenType.Boolean => token1.Value<bool>() == token2.Value<bool>(),
            JTokenType.Null => true,
            _ => false
        };
    }
    
    private bool CompareObject(JToken token1, JToken token2)
    {
        var obj1 = token1 as JObject;
        var obj2 = token2 as JObject;
        
        if (obj1 == null || obj2 == null)
            return false;
            
        if (obj1.Count != obj2.Count)
            return false;
            
        return obj1.Properties().All(prop => 
            obj2.TryGetValue(prop.Name, out var value2) && JTokenEquals(prop.Value, value2));
    }
    
    private bool CompareArray(JToken token1, JToken token2)
    {
        var array1 = token1 as JArray;
        var array2 = token2 as JArray;
        
        if (array1 == null || array2 == null)
            return false;
            
        if (array1.Count != array2.Count)
            return false;
            
        return array1.Select((item, index) => JTokenEquals(item, array2[index])).All(x => x);
    }
} 