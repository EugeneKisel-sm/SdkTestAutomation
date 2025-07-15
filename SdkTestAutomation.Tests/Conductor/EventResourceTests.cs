using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public class EventResourceTests : BaseTest
{
    [Fact]
    public void GetEvent_ShouldReturnEvent()
    {
        var request = new GetEventRequest();
        var response = EventResourceApi.GetEvent(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}