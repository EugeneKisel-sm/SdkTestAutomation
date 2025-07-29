using RestSharp;
using SdkTestAutomation.Api.Orkes.TokenResource.Request;
using SdkTestAutomation.Api.Orkes.TokenResource.Response;
using SdkTestAutomation.Utils.Logging;
using HttpClient = SdkTestAutomation.Core.HttpClient;

namespace SdkTestAutomation.Api.Orkes.TokenResource;

public class TokenResourceApi(ILogger logger) : HttpClient(logger)
{
    public RestResponse<GetTokenResponse> GetToken(GetTokenRequest request)
    {
        return SendGetRequest<GetTokenResponse>(ApiUrl.TokenResource.TokenUrl, request);
    }
}