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
        var eventName = "test_event";
        var parameters = new Dictionary<string, object>
        {
            ["activeOnly"] = true
        };

        var sdkResponse = await ExecuteSdkCallAsync<List<GetEventResponse>>("event", parameters, "get-event-by-name");
        var apiResponse = EventResourceApi.GetEvent(new GetEventByNameRequest { ActiveOnly = true },
            eventName);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
            "SDK response does not match API response");
    }
}