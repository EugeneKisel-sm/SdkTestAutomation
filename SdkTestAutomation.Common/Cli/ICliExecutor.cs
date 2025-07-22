namespace SdkTestAutomation.Common.Cli;

public interface ICliExecutor
{
    Task<CliResult> ExecuteAsync(string command, string arguments, string workingDirectory = null);
    Task<bool> IsAvailableAsync();
    Task<string> GetVersionAsync();
} 