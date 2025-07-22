using System.Diagnostics;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Common.Cli;

public abstract class CliExecutor : ICliExecutor
{
    protected readonly ILogger Logger;
    protected readonly string SdkPath;

    protected CliExecutor(string sdkPath, ILogger logger)
    {
        SdkPath = sdkPath;
        Logger = logger;
    }

    public abstract Task<CliResult> ExecuteAsync(string command, string arguments, string workingDirectory = null);
    
    public virtual async Task<bool> IsAvailableAsync()
    {
        try
        {
            var version = await GetVersionAsync();
            return !string.IsNullOrEmpty(version);
        }
        catch
        {
            return false;
        }
    }

    public abstract Task<string> GetVersionAsync();

    protected CliResult ExecuteProcess(string fileName, string arguments, string workingDirectory = null)
    {
        var startTime = DateTime.UtcNow;
        var fullCommand = $"{fileName} {arguments}";
        
        Logger.Log($"Executing command: {fullCommand}");
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            }
        };

        try
        {
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            var result = new CliResult
            {
                ExitCode = process.ExitCode,
                StandardOutput = output.Trim(),
                StandardError = error.Trim(),
                ExecutionTime = DateTime.UtcNow - startTime,
                RawCommand = fullCommand
            };

            Logger.Log($"Command completed with exit code: {result.ExitCode}");
            if (!string.IsNullOrEmpty(result.StandardError))
            {
                Logger.Log($"Command error output: {result.StandardError}");
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.Log($"Command execution failed: {ex.Message}");
            return new CliResult
            {
                ExitCode = -1,
                StandardError = ex.Message,
                ExecutionTime = DateTime.UtcNow - startTime,
                RawCommand = fullCommand
            };
        }
        finally
        {
            process?.Dispose();
        }
    }
} 