using System.Diagnostics;
using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.Java;

public abstract class BaseJavaClient : ISdkClient
{
    private string _jarPath;
    private bool _initialized;
    
    public bool IsInitialized => _initialized && !string.IsNullOrEmpty(_jarPath);
    
    public void Initialize(string serverUrl)
    {
        try
        {
            _jarPath = FindJarFile();
            if (string.IsNullOrEmpty(_jarPath))
            {
                throw new InvalidOperationException($"{GetJarName()} JAR file not found. Please ensure the JAR files are properly downloaded.");
            }
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Java client: {ex.Message}", ex);
        }
    }
    
    public async Task<string> ExecuteJavaCall(string resource, string operation, object requestData = null)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Java client is not initialized.");
        }
        
        var command = BuildCommand(resource, operation, requestData);
        
        var startInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-jar \"{_jarPath}\" {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start Java process.");
        }
        
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        
        await process.WaitForExitAsync();
        
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Java process failed with exit code {process.ExitCode}: {error}");
        }
        
        return output.Trim();
    }
    
    private string BuildCommand(string resource, string operation, object requestData)
    {
        var parameters = requestData != null ? JsonSerializer.Serialize(requestData) : "{}";
        var parametersJson = parameters.Replace("\"", "\\\"");
        return $"--operation {operation} --parameters \"{parametersJson}\" --resource {resource}";
    }
    
    private string FindJarFile()
    {
        var jarName = GetJarName();
        var possiblePaths = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", jarName),
            Path.Combine(Directory.GetCurrentDirectory(), "lib", jarName),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jarName),
            Path.Combine(Directory.GetCurrentDirectory(), jarName)
        };
        
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }
        
        return null;
    }
    
    protected abstract string GetJarName();
    
    public void Dispose()
    {
        _initialized = false;
    }
} 