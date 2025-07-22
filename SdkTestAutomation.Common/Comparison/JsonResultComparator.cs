using System.Text.Json;

namespace SdkTestAutomation.Common.Comparison;

public class JsonResultComparator : IResultComparator
{
    public Task<ComparisonResult> CompareAsync(string sdkOutput, string restApiResponse)
    {
        try
        {
            // Parse SDK output (may need to extract JSON from CLI output)
            var sdkJson = ExtractJsonFromCliOutput(sdkOutput);
            var sdkObject = JsonSerializer.Deserialize<JsonElement>(sdkJson);
            
            // Parse REST API response
            var restObject = JsonSerializer.Deserialize<JsonElement>(restApiResponse);
            
            // Compare structural equality
            var differences = CompareJsonElements(sdkObject, restObject, "");
            
            return Task.FromResult(new ComparisonResult
            {
                IsEqual = differences.Count == 0,
                Differences = string.Join("\n", differences),
                SdkOutput = sdkOutput,
                RestApiResponse = restApiResponse
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ComparisonResult
            {
                IsEqual = false,
                Differences = $"Error during comparison: {ex.Message}",
                SdkOutput = sdkOutput,
                RestApiResponse = restApiResponse
            });
        }
    }
    
    private string ExtractJsonFromCliOutput(string cliOutput)
    {
        if (string.IsNullOrWhiteSpace(cliOutput))
            return "{}";
            
        // Handle different CLI output formats
        // Some SDKs might output JSON directly, others might have additional formatting
        var lines = cliOutput.Split('\n');
        
        // Look for JSON content (lines starting with { or [)
        var jsonLines = lines.Where(line => 
        {
            var trimmed = line.Trim();
            return trimmed.StartsWith("{") || trimmed.StartsWith("[");
        }).ToList();
        
        if (jsonLines.Count == 0)
        {
            // If no JSON found, try to parse the entire output as JSON
            return cliOutput.Trim();
        }
        
        return string.Join("\n", jsonLines);
    }
    
    private List<string> CompareJsonElements(JsonElement sdk, JsonElement rest, string path)
    {
        var differences = new List<string>();
        
        if (sdk.ValueKind != rest.ValueKind)
        {
            differences.Add($"Type mismatch at {path}: SDK={sdk.ValueKind}, REST={rest.ValueKind}");
            return differences;
        }
        
        switch (sdk.ValueKind)
        {
            case JsonValueKind.Object:
                var sdkProps = sdk.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
                var restProps = rest.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
                
                foreach (var sdkProp in sdkProps)
                {
                    if (!restProps.ContainsKey(sdkProp.Key))
                    {
                        differences.Add($"Missing property in REST response: {path}.{sdkProp.Key}");
                    }
                    else
                    {
                        differences.AddRange(CompareJsonElements(sdkProp.Value, restProps[sdkProp.Key], $"{path}.{sdkProp.Key}"));
                    }
                }
                
                foreach (var restProp in restProps)
                {
                    if (!sdkProps.ContainsKey(restProp.Key))
                    {
                        differences.Add($"Missing property in SDK response: {path}.{restProp.Key}");
                    }
                }
                break;
                
            case JsonValueKind.Array:
                if (sdk.GetArrayLength() != rest.GetArrayLength())
                {
                    differences.Add($"Array length mismatch at {path}: SDK={sdk.GetArrayLength()}, REST={rest.GetArrayLength()}");
                }
                else
                {
                    for (int i = 0; i < sdk.GetArrayLength(); i++)
                    {
                        differences.AddRange(CompareJsonElements(sdk[i], rest[i], $"{path}[{i}]"));
                    }
                }
                break;
                
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                if (sdk.GetRawText() != rest.GetRawText())
                {
                    differences.Add($"Value mismatch at {path}: SDK={sdk.GetRawText()}, REST={rest.GetRawText()}");
                }
                break;
        }
        
        return differences;
    }
} 