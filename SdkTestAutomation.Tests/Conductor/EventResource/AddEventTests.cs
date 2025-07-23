using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class AddEventTests : BaseTest
{
    [Fact]
    public async Task EventResource_AddEvent_200()
    {
        var parameters = new Dictionary<string, object>
        {
            ["name"] = $"test_event_sdk_{Guid.NewGuid():N}",
            ["event"] = "test_event",
            ["active"] = true
        };

        var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("event", parameters, "add-event");
        var apiResponse = EventResourceApi.AddEvent(
            new AddEventRequest
            {
                Name = $"test_event_api_{Guid.NewGuid():N}",
                Event = "test_event",
                Actions = new List<EventAction>(),
                Active = true
            });

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
            "SDK response does not match API response");
    }
}