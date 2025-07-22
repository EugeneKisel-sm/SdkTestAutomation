using SdkTestAutomation.Common.Cli;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Python;

public class PythonSdkExecutor : CliExecutor
{
    public PythonSdkExecutor(string sdkPath, ILogger logger) : base(sdkPath, logger)
    {
    }

    public override Task<CliResult> ExecuteAsync(string command, string arguments, string workingDirectory = null)
    {
        var fullArguments = $"-m conductor_sdk {command} {arguments}";
        return Task.FromResult(ExecuteProcess("python", fullArguments, workingDirectory ?? SdkPath));
    }

    public override Task<string> GetVersionAsync()
    {
        var result = ExecuteProcess("python", "--version");
        return Task.FromResult(result.IsSuccess ? result.StandardOutput : string.Empty);
    }
} 