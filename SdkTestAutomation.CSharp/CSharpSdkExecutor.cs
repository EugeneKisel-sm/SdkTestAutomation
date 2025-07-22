using SdkTestAutomation.Common.Cli;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.CSharp;

public class CSharpSdkExecutor : CliExecutor
{
    public CSharpSdkExecutor(string sdkPath, ILogger logger) : base(sdkPath, logger)
    {
    }

    public override Task<CliResult> ExecuteAsync(string command, string arguments, string workingDirectory = null)
    {
        var fullArguments = $"run --project {SdkPath} {command} {arguments}";
        return Task.FromResult(ExecuteProcess("dotnet", fullArguments, workingDirectory));
    }

    public override Task<string> GetVersionAsync()
    {
        var result = ExecuteProcess("dotnet", "--version");
        return Task.FromResult(result.IsSuccess ? result.StandardOutput : string.Empty);
    }
} 