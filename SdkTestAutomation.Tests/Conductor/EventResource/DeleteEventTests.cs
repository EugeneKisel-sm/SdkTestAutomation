using System.Net;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class DeleteEventTests : BaseTest
{
    [Fact]
    public void EventResource_DeleteEvent_200()
    {
        var eventName = $"test_event_delete_{Guid.NewGuid():N}";
        
        // First add an event
        var addResponse = EventAdapter.AddEvent(eventName, "test_event", true);
        Assert.True(addResponse.Success, "Failed to add event for delete test");
        
        // Then delete it
        var sdkResponse = EventAdapter.DeleteEvent(eventName);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
    }
}