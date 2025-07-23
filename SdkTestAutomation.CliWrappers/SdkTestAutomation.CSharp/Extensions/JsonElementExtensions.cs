using System.Text.Json;

namespace SdkTestAutomation.CSharp.Extensions;

public static class JsonElementExtensions
{
    public static string GetString(this Dictionary<string, JsonElement> parameters, string key, string defaultValue = "")
    {
        return parameters.TryGetValue(key, out var element) ? element.GetString() ?? defaultValue : defaultValue;
    }
    
    public static bool GetBool(this Dictionary<string, JsonElement> parameters, string key, bool defaultValue = false)
    {
        return parameters.TryGetValue(key, out var element) && element.ValueKind == JsonValueKind.True;
    }
    
    public static bool? GetBoolNullable(this Dictionary<string, JsonElement> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var element))
            return null;
            
        return element.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };
    }
} 