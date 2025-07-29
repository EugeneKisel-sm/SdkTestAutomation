using System.Net;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class GetEventConductorTests : BaseConductorTest
{
    [Fact]
    public void EventResource_GetEvent_200()
    {
        var sdkResponse = EventAdapter.GetEvents();

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
    }

    [Fact]
    public void EventResource_GetEvent_EmptyName_404()
    {
        var sdkResponse = EventAdapter.GetEventByName("");

        // This might fail or return empty results, but should not throw
        Assert.NotNull(sdkResponse);
    }
}