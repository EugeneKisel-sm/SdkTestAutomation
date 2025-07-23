using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class DeleteEventTests : BaseTest
{
    [Fact]
    public async Task EventResource_DeleteEvent_200()
    {
        var eventName = $"test_event_delete_{Guid.NewGuid():N}";

        await ExecuteSdkCallAsync<GetEventResponse>("event", new Dictionary<string, object>
        {
            ["name"] = eventName,
            ["event"] = "test_event",
            ["active"] = true
        }, "add-event");

        var sdkResponse = await ExecuteSdkCallAsync<object>("event", new Dictionary<string, object>
        {
            ["name"] = eventName
        }, "delete-event");

        var apiResponse = EventResourceApi.GetEvent(new GetEventByNameRequest(), eventName);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
    }
}