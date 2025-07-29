using SdkTestAutomation.Sdk.Core;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;
using SdkTestAutomation.Api.Orkes.TokenResource;
using Xunit;

namespace SdkTestAutomation.Tests.Orkes;

public abstract class BaseOrkesTest : IDisposable
{
    private readonly ILogger _logger;

    #region Sdk

    protected ITokenAdapter TokenAdapter { get; }

    #endregion

    #region OrkesApi

    protected TokenResourceApi TokenResourceApi { get; }

    #endregion
    
    protected BaseOrkesTest()
    {
        var testContext = TestContext.Current;
        _logger = new ConsoleLogger(testContext);
        _logger.Log($"Using SDK type: {TestConfig.SdkType}");
        
        TokenAdapter = SdkFactory.CreateTokenAdapter(TestConfig.SdkType);
        
        var serverUrl = TestConfig.ApiUrl;
        TokenAdapter.Initialize(serverUrl);
        
        TokenResourceApi = new TokenResourceApi(_logger);
        
        _logger.Log($"Test '{testContext.TestCase?.TestCaseDisplayName}' execution started.");
    }
    
    public virtual void Dispose()
    {
        if (TestConfig.SdkType == "go")
        {
            LogGoSdkDetails();
        }
        
        TokenAdapter?.Dispose();
        _logger.Log($"Test '{TestContext.Current.TestCase?.TestCaseDisplayName}' completed.");
    }
    
    private void LogGoSdkDetails()
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            _logger.Log($"Error retrieving Go SDK logs: {ex.Message}");
        }
    }
} 