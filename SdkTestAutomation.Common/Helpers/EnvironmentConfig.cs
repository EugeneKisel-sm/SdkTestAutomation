using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Helper for loading environment configuration from .env files
/// </summary>
public static class EnvironmentConfig
{
    private static readonly ILogger _logger = new ConsoleLogger(null);
    
    /// <summary>
    /// Load environment variables from .env file
    /// </summary>
    public static void LoadEnvironmentFile()
    {
        var envPath = Path.Combine(GetProjectRoot(), "SdkTestAutomation.Tests/.env");
        if (File.Exists(envPath))
        {
            _logger.Log($"Loading environment from: {envPath}");
            foreach (var line in File.ReadAllLines(envPath))
            {
                if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        Environment.SetEnvironmentVariable(key, value);
                        _logger.Log($"Set environment variable: {key}={value}");
                    }
                }
            }
        }
        else
        {
            _logger.Log($"Environment file not found: {envPath}");
        }
    }
    
    /// <summary>
    /// Get the project root directory
    /// </summary>
    public static string GetProjectRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (!File.Exists(Path.Combine(dir, "SdkTestAutomation.sln")))
        {
            var parent = Directory.GetParent(dir);
            if (parent == null)
            {
                throw new Exception("Project root not found. Make sure you're running from within the SdkTestAutomation directory.");
            }
            dir = parent.FullName;
        }
        return dir;
    }
    
    /// <summary>
    /// Get environment variable with fallback
    /// </summary>
    public static string GetEnvironmentVariable(string key, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
    
    /// <summary>
    /// Get required environment variable
    /// </summary>
    public static string GetRequiredEnvironmentVariable(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception($"Required environment variable '{key}' is not set.");
        }
        return value;
    }
} 