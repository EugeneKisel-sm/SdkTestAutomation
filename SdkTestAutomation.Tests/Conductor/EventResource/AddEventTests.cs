using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class AddEventTests : BaseTest
{
    [Fact]
    public void EventResource_AddEvent_200()
    {
        var eventName = $"test_event_sdk_{Guid.NewGuid():N}";
        var request = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event",
            Actions = new List<EventAction>(),
            Active = true
        };

        var sdkResponse = EventResourceAdapter.AddEvent(request);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
        
        var apiRequest = new AddEventRequest
        {
            Name = "test_event_add",
            Event = "test_event",
            Actions = new List<EventAction>
            {
            },
            Active = true
        };
        
        var apiResponse = EventResourceApi.AddEvent(apiRequest);
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
    }
}