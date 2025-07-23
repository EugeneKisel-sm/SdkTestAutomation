using System.Diagnostics;
using System.Text.Json;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Sdk;

public class SdkCommandExecutor
{
    private readonly ILogger _logger;
    
    public SdkCommandExecutor(ILogger logger)
    {
        _logger = logger;
    }
    
    public async Task<SdkResponse<T>> ExecuteAsync<T>(string operation, Dictionary<string, object> parameters, string resource)
    {
        var command = BuildCommand(operation, parameters, resource);
        return await ExecuteCommandAsync<T>(command);
    }
    
    private string BuildCommand(string operation, Dictionary<string, object> parameters, string resource)
    {
        var parametersJson = JsonSerializer.Serialize(parameters).Replace("\"", "\\\"");
        return $"--operation {operation} --parameters \"{parametersJson}\" --resource {resource}";
    }
    
    private async Task<SdkResponse<T>> ExecuteCommandAsync<T>(string command)
    {
        _logger.Log($"Executing SDK command: {command}");
        
        var (fileName, arguments) = GetProcessInfo(command);
        
        _logger.Log($"Starting process with FileName: {fileName}");
        _logger.Log($"Starting process with Arguments: {arguments}");
        
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        _logger.Log("Starting process...");
        process.Start();
        _logger.Log($"Process started with ID: {process.Id}");
        
        _logger.Log("Reading process output...");
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        
        _logger.Log("Waiting for process to exit...");
        await process.WaitForExitAsync();
        
        _logger.Log($"Process exit code: {process.ExitCode}");
        _logger.Log($"Process output length: {output?.Length ?? 0} characters");
        _logger.Log($"Process error length: {error?.Length ?? 0} characters");
        
        if (!string.IsNullOrEmpty(output))
            _logger.Log($"Process output: {output}");
        if (!string.IsNullOrEmpty(error))
            _logger.Log($"Process error: {error}");
        
        if (process.ExitCode != 0)
        {
            return new SdkResponse<T>
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = process.ExitCode
            };
        }

        return DeserializeResponse<T>(output);
    }
    
    private SdkResponse<T> DeserializeResponse<T>(string output)
    {
        var cleanedOutput = CleanJsonOutput(output);
        _logger.Log($"Cleaned Output: [{cleanedOutput}]");
        
        try
        {
            var response = JsonSerializer.Deserialize<SdkResponse<T>>(cleanedOutput, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            return response ?? new SdkResponse<T> { Success = false, ErrorMessage = "Failed to deserialize response" };
        }
        catch (JsonException ex)
        {
            _logger.Log($"Error deserializing response: {ex.Message}");
            return new SdkResponse<T>
            {
                Success = false,
                ErrorMessage = $"JSON deserialization error: {ex.Message}",
                StatusCode = 500
            };
        }
    }
    
    private string CleanJsonOutput(string output)
    {
        var trimmed = output.Trim();
        if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
            return trimmed;
            
        var lines = trimmed.Split('\n');
        return lines.FirstOrDefault(line => line.Trim().StartsWith("{") && line.Trim().EndsWith("}")) ?? trimmed;
    }
    
    private (string fileName, string arguments) GetProcessInfo(string command)
    {
        var sdkType = TestConfig.SdkType.ToLowerInvariant();
        var projectRoot = GetProjectRoot();
        _logger.Log($"Getting process info for SDK type: {sdkType}");
        _logger.Log($"Project root: {projectRoot}");
        _logger.Log($"Command: {command}");
        
        var (fileName, arguments) = sdkType switch
        {
            "csharp" => (Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/bin/Debug/net8.0/SdkTestAutomation.CSharp"), command),
            "java" => ("java", $"-jar \"{Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar")}\" {command}"),
            "python" => (Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/venv/bin/python"), $"{Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/sdk_wrapper/main.py")} {command}"),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
        
        _logger.Log($"Process info - FileName: {fileName}");
        _logger.Log($"Process info - Arguments: {arguments}");
        
        return (fileName, arguments);
    }
    
    private string GetProjectRoot()
    {
        var currentDir = Directory.GetCurrentDirectory();
        _logger.Log($"Current directory: {currentDir}");
        
        // If we're in the test project's bin directory, go up to the project root
        if (currentDir.Contains("SdkTestAutomation.Tests/bin"))
        {
            var projectRoot = Path.GetFullPath(Path.Combine(currentDir, "../../../.."));
            _logger.Log($"Found project root (from tests bin): {projectRoot}");
            return projectRoot;
        }
        
        // If we're in the project root, return it
        if (File.Exists(Path.Combine(currentDir, "SdkTestAutomation.sln")))
        {
            _logger.Log($"Found project root (current directory): {currentDir}");
            return currentDir;
        }
        
        // Try to find the project root by looking for the solution file
        var dir = new DirectoryInfo(currentDir);
        while (dir.Parent != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "SdkTestAutomation.sln")))
            {
                _logger.Log($"Found project root (parent search): {dir.FullName}");
                return dir.FullName;
            }
            dir = dir.Parent;
        }
        
        _logger.Log($"Using current directory as project root: {currentDir}");
        return currentDir;
    }
} 