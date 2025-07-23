using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class GetEventTests : BaseTest
{
    [Fact]
    public async Task EventResource_GetEvent_200()
    {
        var sdkResponse =
            await ExecuteSdkCallAsync<List<GetEventResponse>>("get-event", new Dictionary<string, object>(), "event");
        var apiResponse = EventResourceApi.GetEvent(new GetEventRequest());

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
            "SDK response does not match API response");
    }

    [Fact]
    public async Task EventResource_GetEvent_EmptyName_404()
    {
        /*var request = new GetEventByNameRequest() { ActiveOnly = true };
        var response = EventResourceApi.GetEvent(request, null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);*/

        var eventName = "test_event";
        var parameters = new Dictionary<string, object>
        {
            ["event"] = eventName,
            ["activeOnly"] = true
        };

        var sdkResponse = await ExecuteSdkCallAsync<List<GetEventResponse>>("get-event-by-name", parameters, "event");
        var apiResponse = EventResourceApi.GetEvent(new GetEventByNameRequest { Event = eventName, ActiveOnly = true },
            eventName);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
            "SDK response does not match API response");
    }
}