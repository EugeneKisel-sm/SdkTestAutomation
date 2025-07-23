using Newtonsoft.Json.Linq;

namespace SdkTestAutomation.CSharp.Extensions;

public static class DictionaryExtensions
{
    public static string GetString(this Dictionary<string, JToken> parameters, string key, string defaultValue = "")
    {
        return parameters.TryGetValue(key, out var token) ? token?.ToString() ?? defaultValue : defaultValue;
    }
    
    public static bool GetBool(this Dictionary<string, JToken> parameters, string key)
    {
        return parameters.TryGetValue(key, out var token) && token?.Type == JTokenType.Boolean && token.Value<bool>();
    }
    
    public static bool? GetBoolNullable(this Dictionary<string, JToken> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var token))
            return null;
            
        return token?.Type == JTokenType.Boolean ? token.Value<bool>() : null;
    }
} 