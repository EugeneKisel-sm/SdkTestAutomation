using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Implementations.CSharp;
using SdkTestAutomation.Sdk.Implementations.Java.Conductor;
using SdkTestAutomation.Sdk.Implementations.Python;
using SdkTestAutomation.Sdk.Implementations.Go;
using SdkTestAutomation.Sdk.Implementations.Java.Orkes;

namespace SdkTestAutomation.Sdk.Core;

public static class SdkFactory
{
    public static ITokenAdapter CreateTokenAdapter(string sdkType)
    {
        return sdkType.ToLowerInvariant() switch
        {
            "csharp" => new CSharpTokenAdapter(),
            "java" => new JavaTokenAdapter(),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
    }

    public static IEventAdapter CreateEventAdapter(string sdkType)
    {
        return sdkType.ToLowerInvariant() switch
        {
            "csharp" => new CSharpEventAdapter(),
            "java" => new JavaEventAdapter(),
            "python" => new PythonEventAdapter(),
            "go" => new GoEventAdapter(),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
    }
    
    public static IWorkflowAdapter CreateWorkflowAdapter(string sdkType)
    {
        return sdkType.ToLowerInvariant() switch
        {
            "csharp" => new CSharpWorkflowAdapter(),
            "java" => new JavaWorkflowAdapter(),
            "python" => new PythonWorkflowAdapter(),
            "go" => new GoWorkflowAdapter(),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
    }
} 