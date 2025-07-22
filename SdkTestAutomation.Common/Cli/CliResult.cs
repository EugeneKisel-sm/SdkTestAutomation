namespace SdkTestAutomation.Common.Cli;

public class CliResult
{
    public int ExitCode { get; set; }
    public string StandardOutput { get; set; } = string.Empty;
    public string StandardError { get; set; } = string.Empty;
    public TimeSpan ExecutionTime { get; set; }
    public string RawCommand { get; set; } = string.Empty;
    public bool IsSuccess => ExitCode == 0;
    
    public override string ToString()
    {
        return $"Command: {RawCommand}\nExit Code: {ExitCode}\nOutput: {StandardOutput}\nError: {StandardError}\nExecution Time: {ExecutionTime}";
    }
} 