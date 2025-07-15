using RestSharp;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Utils.Logging;
using HttpClient = SdkTestAutomation.Core.HttpClient;

namespace SdkTestAutomation.Api.Conductor.EventResource
{
    public class EventResourceApi(ILogger logger) : HttpClient(logger)
    {
        public RestResponse<GetEventResponse> GetEvent(GetEventRequest request)
        {
            return SendGetRequest<GetEventResponse>(ApiUrl.EventResource.EventUrl, request);
        }
    }
}
