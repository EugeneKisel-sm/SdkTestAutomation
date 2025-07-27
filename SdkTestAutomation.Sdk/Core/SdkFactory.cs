using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Implementations.CSharp;
using SdkTestAutomation.Sdk.Implementations.Java;
using SdkTestAutomation.Sdk.Implementations.Python;
using SdkTestAutomation.Sdk.Implementations.Go;

namespace SdkTestAutomation.Sdk.Core;

public static class SdkFactory
{
    public static IEventAdapter CreateEventAdapter(string sdkType)
    {
        return sdkType.ToLowerInvariant() switch
        {
            "csharp" => new CSharpEventAdapter(),
            "java" => new JavaEventAdapter(),
            "python" => new PythonEventAdapter(),
            "go" => new GoSharedLibraryEventAdapter(), // Use shared library approach
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
            "go" => new GoSharedLibraryWorkflowAdapter(), // Use shared library approach
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
    }
} 