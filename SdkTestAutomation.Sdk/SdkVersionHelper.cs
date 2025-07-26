using System.Reflection;

namespace SdkTestAutomation.Sdk;

public static class SdkVersionHelper
{
    public static string GetAssemblyVersion(Type type)
    {
        try
        {
            var assembly = type.Assembly;
            var version = assembly.GetName().Version?.ToString();
            return version ?? "unknown";
        }
        catch
        {
            return "unknown";
        }
    }
    
    public static string GetTypeVersion(string typeName, string assemblyName)
    {
        try
        {
            var type = Type.GetType($"{typeName}, {assemblyName}");
            if (type != null)
            {
                var version = type.Assembly.GetName().Version?.ToString();
                if (!string.IsNullOrEmpty(version))
                    return version;
            }
            return "unknown";
        }
        catch
        {
            return "unknown";
        }
    }
    
    public static string GetModuleVersion(Func<dynamic> getModule)
    {
        try
        {
            dynamic module = getModule();
            return (string)module.__version__;
        }
        catch
        {
            return "unknown";
        }
    }
} 