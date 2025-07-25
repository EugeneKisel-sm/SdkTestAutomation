using DotNetEnv;

namespace SdkTestAutomation.Utils;

public static class TestConfig
{
    public static string Key { get; }
    public static string Secret { get; }
    public static string ApiUrl { get; }
    public static string SdkType { get; }

    static TestConfig()
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }

        ApiUrl = GetRequired("CONDUCTOR_SERVER_URL");
        Key = GetOptional("CONDUCTOR_AUTH_KEY", "");
        Secret = GetOptional("CONDUCTOR_AUTH_SECRET", "");
        SdkType = GetRequired("TEST_SDK");
    }

    private static string GetRequired(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Required environment variable '{name}' is missing.");
        }

        return value;
    }

    private static string GetOptional(string name, string @default) =>
        string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name))
            ? @default
            : Environment.GetEnvironmentVariable(name)!;
    
    public static string GetEnvironmentVariable(string key, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
}