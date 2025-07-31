using System.Diagnostics;
using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.Java.Orkes;

public class JavaClient : ISdkClient
{
    private string _serverUrl;
    private string _javaExecutable;
    private string _jarPath;
    private bool _initialized;
    
    public bool IsInitialized => _initialized && !string.IsNullOrEmpty(_javaExecutable) && !string.IsNullOrEmpty(_jarPath);
    
    public void Initialize(string serverUrl)
    {
        try
        {
            _serverUrl = serverUrl;
            
            // Find Java executable
            _javaExecutable = FindJavaExecutable();
            if (string.IsNullOrEmpty(_javaExecutable))
            {
                throw new InvalidOperationException("Java executable not found. Please ensure Java 17+ is installed and in PATH.");
            }
            
            // Find JAR file
            _jarPath = FindOrkesJar();
            if (string.IsNullOrEmpty(_jarPath))
            {
                throw new InvalidOperationException("Orkes Conductor JAR file not found. Please ensure the JAR files are properly downloaded.");
            }
            
            // Test Java connection
            TestJavaConnection();
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Java client: {ex.Message}", ex);
        }
    }
    
    public string ExecuteJavaCall(string resource, string operation, object requestData = null)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Java client is not initialized.");
        }
        
        try
        {
            var parameters = requestData != null ? JsonSerializer.Serialize(requestData) : "{}";
            
            var startInfo = new ProcessStartInfo
            {
                FileName = _javaExecutable,
                Arguments = $"-jar \"{_jarPath}\" --resource {resource} --operation {operation} --parameters \"{parameters}\"",
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
            
            process.WaitForExit(30000); // 30 second timeout
            
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Java process failed with exit code {process.ExitCode}: {error}");
            }
            
            return output.Trim();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to execute Java call: {ex.Message}", ex);
        }
    }
    
    private string FindJavaExecutable()
    {
        var javaNames = new[] { "java", "java.exe" };
        
        foreach (var javaName in javaNames)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = javaName,
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    var output = process.StandardError.ReadToEnd(); // Java version goes to stderr
                    process.WaitForExit(5000);
                    
                    if (process.ExitCode == 0 && output.Contains("version"))
                    {
                        // Check if it's Java 17 or higher
                        if (output.Contains("version \"17") || output.Contains("version \"18") || 
                            output.Contains("version \"19") || output.Contains("version \"20") ||
                            output.Contains("version \"21") || output.Contains("version \"22"))
                        {
                            return javaName;
                        }
                    }
                }
            }
            catch
            {
                // Continue to next option
            }
        }
        
        return null;
    }
    
    private string FindOrkesJar()
    {
        var possiblePaths = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orkes-conductor-client.jar"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "orkes-conductor-client.jar"),
            Path.Combine(Directory.GetCurrentDirectory(), "orkes-conductor-client.jar"),
            Path.Combine(Directory.GetCurrentDirectory(), "lib", "orkes-conductor-client.jar"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "SdkTestAutomation.Sdk", "bin", "Debug", "net8.0", "lib", "orkes-conductor-client.jar")
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
    
    private void TestJavaConnection()
    {
        try
        {
            var result = ExecuteJavaCall("token", "generate-token", new { keyId = "test", keySecret = "test" });
            if (string.IsNullOrEmpty(result))
            {
                throw new InvalidOperationException("Java connection test failed - no response received.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Java connection test failed: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _initialized = false;
    }
} 