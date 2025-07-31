using System.Net;
using SdkTestAutomation.Api.Orkes.TokenResource.Request;
using SdkTestAutomation.Utils;
using Xunit;

namespace SdkTestAutomation.Tests.Orkes.TokenResource;

public class GetTokenTests : BaseOrkesTest
{
    [Fact]
    [Trait(TraitName.Category, TestType.Orkes)]
    public void TokenResource_GetToken_200()
    { 
        var sdkResponse = TokenAdapter.GenerateToken(TestConfig.Key, TestConfig.Secret);

        var apiResponse = TokenResourceApi.GetToken(new GetTokenRequest()
            { KeyId = TestConfig.Key, KeySecret = TestConfig.Secret });

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, sdkResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.NotNull(sdkResponse.Content);
        Assert.NotNull(apiResponse.Data.Token);
    }
}