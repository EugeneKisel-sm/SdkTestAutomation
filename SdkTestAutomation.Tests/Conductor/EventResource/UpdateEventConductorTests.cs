using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class UpdateEventConductorTests : BaseConductorTest
{
    [Fact]
    [Trait(TraitName.Category, TestType.Conductor)]
    public void EventResource_UpdateEvent_200()
    {
        var eventName = $"test_event_update_{Guid.NewGuid():N}";
        
        // First add an event
        var addResponse = EventAdapter.AddEvent(eventName, "test_event", true);
        Assert.True(addResponse.Success, "Failed to add event for update test");
        
        // Then update it
        var sdkResponse = EventAdapter.UpdateEvent(eventName, "test_event_updated", false);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, sdkResponse.StatusCode);
    }
}