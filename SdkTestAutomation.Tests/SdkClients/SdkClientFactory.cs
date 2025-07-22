using SdkTestAutomation.Common.Cli;
using SdkTestAutomation.Common.Configuration;
using SdkTestAutomation.CSharp;
using SdkTestAutomation.Java;
using SdkTestAutomation.Python;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Tests.SdkClients;

public static class SdkClientFactory
{
    public static ICliExecutor CreateExecutor(SdkType sdkType, ILogger logger)
    {
        var sdkPath = GetSdkPath(sdkType);
        
        return sdkType switch
        {
            SdkType.CSharp => new CSharpSdkExecutor(sdkPath, logger),
            SdkType.Java => new JavaSdkExecutor(sdkPath, logger),
            SdkType.Python => new PythonSdkExecutor(sdkPath, logger),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
    }
    
    private static string GetSdkPath(SdkType sdkType)
    {
        var sdkSource = Environment.GetEnvironmentVariable("SDK_SOURCE") ?? "Package";
        
        return sdkSource.ToLower() switch
        {
            "package" => GetPackageSdkPath(sdkType),
            "local" => GetLocalSdkPath(sdkType),
            _ => throw new ArgumentException($"Unsupported SDK source: {sdkSource}")
        };
    }
    
    private static string GetPackageSdkPath(SdkType sdkType)
    {
        return sdkType switch
        {
            SdkType.CSharp => Environment.GetEnvironmentVariable("CSHARP_SDK_PATH") ?? "./sdks/csharp-sdk",
            SdkType.Java => Environment.GetEnvironmentVariable("JAVA_SDK_PATH") ?? "./sdks/java-sdk.jar",
            SdkType.Python => Environment.GetEnvironmentVariable("PYTHON_SDK_PATH") ?? "./sdks/python-sdk",
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
    }
    
    private static string GetLocalSdkPath(SdkType sdkType)
    {
        var localPath = Environment.GetEnvironmentVariable("SDK_PATH");
        if (string.IsNullOrEmpty(localPath))
        {
            throw new InvalidOperationException("SDK_PATH environment variable is required for local SDK builds");
        }
        
        return sdkType switch
        {
            SdkType.CSharp => Path.Combine(localPath, "csharp-sdk"),
            SdkType.Java => Path.Combine(localPath, "java-sdk", "build", "libs", "conductor-sdk.jar"),
            SdkType.Python => Path.Combine(localPath, "python-sdk"),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
    }
} 