using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Common.Configuration;

public class SdkConfiguration
{
    public SdkType SdkType { get; set; }
    public string SdkPath { get; set; }
    public string SdkSource { get; set; } = "Package"; // Package or Local
    public ILogger Logger { get; set; }
    
    public SdkConfiguration(SdkType sdkType, string sdkPath, ILogger logger)
    {
        SdkType = sdkType;
        SdkPath = sdkPath;
        Logger = logger;
    }
} 