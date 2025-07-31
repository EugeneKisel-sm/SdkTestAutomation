using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class AddEventConductorTests : BaseConductorTest
{
    [Fact]
    [Trait(TraitName.Category, TestType.Conductor)]
    public void EventResource_AddEvent_200()
    {
        var eventName = $"test_event_sdk_{Guid.NewGuid():N}";
        
        // Test SDK call
        var sdkResponse = EventAdapter.AddEvent(eventName, "test_event", true);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, sdkResponse.StatusCode);
        
        // Test API call for comparison
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