using RestSharp;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Utils.Logging;
using HttpClient = SdkTestAutomation.Core.HttpClient;

namespace SdkTestAutomation.Api.Conductor.EventResource
{
    public class EventResourceApi : HttpClient
    {
        public EventResourceApi(ILogger logger) : base(logger)
        {
        }

        public RestResponse<List<GetEventResponse>> GetEvent(GetEventRequest request)
        {
            return SendGetRequest<List<GetEventResponse>>(ApiUrl.EventResource.EventUrl, request);
        }
        
        public RestResponse<List<GetEventResponse>> GetEvent(GetEventByNameRequest request, string eventName)
        {
            return SendGetRequest<List<GetEventResponse>>(ApiUrl.EventResource.EventUrl + $"/{eventName}", request);
        }
        
        public RestResponse<GetEventResponse> AddEvent(AddEventRequest request)
        {
            return SendPostRequest<GetEventResponse>(ApiUrl.EventResource.EventUrl, request);
        }
        
        public RestResponse<GetEventResponse> UpdateEvent(AddEventRequest request)
        {
            return SendPutRequest<GetEventResponse>(ApiUrl.EventResource.EventUrl, request);
        }
        
        public RestResponse<object> DeleteEvent(DeleteEventRequest request, string eventName)
        {
            return SendDeleteRequest<object>(ApiUrl.EventResource.EventUrl + $"/{eventName}", request);
        }
    }
}
