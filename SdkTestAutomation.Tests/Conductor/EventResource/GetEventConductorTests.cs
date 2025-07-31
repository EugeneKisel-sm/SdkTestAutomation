using System.Net;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class GetEventConductorTests : BaseConductorTest
{
    [Fact]
    [Trait(TraitName.Category, TestType.Conductor)]
    public void EventResource_GetEvent_200()
    {
        var sdkResponse = EventAdapter.GetEvents();

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, sdkResponse.StatusCode);
    }

    [Fact]
    [Trait(TraitName.Category, TestType.Conductor)]
    public void EventResource_GetEvent_EmptyName_404()
    {
        var sdkResponse = EventAdapter.GetEventByName("");

        Assert.NotNull(sdkResponse);
    }
}