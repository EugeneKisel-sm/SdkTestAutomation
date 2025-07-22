using RestSharp;
using SdkTestAutomation.Api.Conductor;
using SdkTestAutomation.Common.Cli;
using SdkTestAutomation.Common.Comparison;
using SdkTestAutomation.Common.Configuration;
using SdkTestAutomation.Tests.SdkClients;
using SdkTestAutomation.Utils.Logging;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public abstract class BaseSdkTest : BaseTest
{
    protected ICliExecutor SdkExecutor { get; }
    protected IResultComparator ResultComparator { get; }
    protected SdkType CurrentSdkType { get; }
    
    protected BaseSdkTest() : base()
    {
        CurrentSdkType = GetSdkTypeFromEnvironment();
        var logger = new ConsoleLogger(TestContext.Current);
        SdkExecutor = SdkClientFactory.CreateExecutor(CurrentSdkType, logger);
        ResultComparator = new JsonResultComparator();
    }
    
    private SdkType GetSdkTypeFromEnvironment()
    {
        var sdkTypeStr = Environment.GetEnvironmentVariable("SDK_TYPE") ?? "CSharp";
        return Enum.Parse<SdkType>(sdkTypeStr, true);
    }
    
    protected async Task ValidateSdkResponseAsync<T>(
        string sdkCommand, 
        string sdkArgs, 
        Func<Task<RestResponse<T>>> restApiCall,
        string testDescription = null)
    {
        Logger.Log($"Starting SDK comparison test: {testDescription ?? sdkCommand}");
        
        // Execute SDK command
        var sdkResult = await SdkExecutor.ExecuteAsync(sdkCommand, sdkArgs);
        Logger.Log($"SDK command executed: {sdkResult.RawCommand}");
        Logger.Log($"SDK exit code: {sdkResult.ExitCode}");
        Logger.Log($"SDK output: {sdkResult.StandardOutput}");
        
        if (!sdkResult.IsSuccess)
        {
            Logger.Log($"SDK error: {sdkResult.StandardError}");
            Assert.Fail($"SDK command failed with exit code {sdkResult.ExitCode}: {sdkResult.StandardError}");
        }
        
        // Execute REST API call
        var restResult = await restApiCall();
        Logger.Log($"REST API status: {restResult.StatusCode}");
        Logger.Log($"REST API response: {restResult.Content}");
        
        // Compare results
        var comparison = await ResultComparator.CompareAsync(
            sdkResult.StandardOutput, 
            restResult.Content);
            
        Logger.Log($"Comparison result: {(comparison.IsEqual ? "PASS" : "FAIL")}");
        if (!comparison.IsEqual)
        {
            Logger.Log($"Differences found: {comparison.Differences}");
        }
        
        Assert.True(comparison.IsEqual, 
            $"SDK and REST API responses differ: {comparison.Differences}");
    }
    

} 