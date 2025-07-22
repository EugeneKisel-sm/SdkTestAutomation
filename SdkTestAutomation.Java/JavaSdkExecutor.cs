using SdkTestAutomation.Common.Cli;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Java;

public class JavaSdkExecutor : CliExecutor
{
    public JavaSdkExecutor(string sdkPath, ILogger logger) : base(sdkPath, logger)
    {
    }

    public override Task<CliResult> ExecuteAsync(string command, string arguments, string workingDirectory = null)
    {
        var fullArguments = $"-jar {SdkPath} {command} {arguments}";
        return Task.FromResult(ExecuteProcess("java", fullArguments, workingDirectory));
    }

    public override Task<string> GetVersionAsync()
    {
        var result = ExecuteProcess("java", "-version");
        return Task.FromResult(result.IsSuccess ? result.StandardError : string.Empty); // Java version goes to stderr
    }
} 