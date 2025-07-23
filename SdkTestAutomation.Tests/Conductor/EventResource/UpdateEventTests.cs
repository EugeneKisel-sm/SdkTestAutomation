using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class UpdateEventTests : BaseTest
{
    [Fact]
    public async Task EventResource_UpdateEvent_200()
    {
        var eventName = $"test_event_update_{Guid.NewGuid():N}";

        await ExecuteSdkCallAsync<GetEventResponse>("event", new Dictionary<string, object>
        {
            ["name"] = eventName,
            ["event"] = "test_event",
            ["active"] = true
        }, "add-event");

        var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("event", new Dictionary<string, object>
        {
            ["name"] = eventName,
            ["event"] = "test_event",
            ["active"] = false
        }, "update-event");

        var apiResponse = EventResourceApi.UpdateEvent(new AddEventRequest
        {
            Name = eventName,
            Event = "test_event",
            Actions = new List<EventAction>(),
            Active = false
        });

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
            "SDK response does not match API response");
    }
}