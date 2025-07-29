using SdkTestAutomation.Api.Orkes.TokenResource.Request;
using SdkTestAutomation.Utils;
using Xunit;

namespace SdkTestAutomation.Tests.Orkes.TokenResource;

public class GetTokenTests : BaseOrkesTest
{
    [Fact]
    public void EventResource_GetEvent_200()
    {
        var sdkResponse = TokenAdapter.GenerateToken(TestConfig.Key, TestConfig.Secret);

        var apiResponse = TokenResourceApi.GetToken(new GetTokenRequest()
            { KeyId = TestConfig.Key, KeySecret = TestConfig.Secret });

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
    }
}