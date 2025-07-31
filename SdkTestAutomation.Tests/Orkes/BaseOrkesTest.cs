using SdkTestAutomation.Sdk.Core;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Implementations.Java.Orkes;
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
        // Log SDK details based on type
        switch (TestConfig.SdkType)
        {
            case "go":
                LogGoSdkDetails();
                break;
            case "java":
                LogJavaSdkDetails();
                break;
        }
        
        TokenAdapter?.Dispose();
        _logger.Log($"Test '{TestContext.Current.TestCase?.TestCaseDisplayName}' completed.");
    }
    
    private void LogGoSdkDetails()
    {
        try
        {
            // Go SDK logging would go here if needed
        }
        catch (Exception ex)
        {
            _logger.Log($"Error retrieving Go SDK logs: {ex.Message}");
        }
    }
    
    private void LogJavaSdkDetails()
    {
        try
        {
            _logger.Log("=== JAVA SDK DETAILS ===");
            _logger.Log($"Token Adapter Type: {TokenAdapter.GetType().Name}");
            _logger.Log($"Token Adapter SDK Type: {TokenAdapter.SdkType}");
            _logger.Log("=== END JAVA SDK DETAILS ===");
        }
        catch (Exception ex)
        {
            _logger.Log($"Error retrieving Java SDK details: {ex.Message}");
        }
    }
} 