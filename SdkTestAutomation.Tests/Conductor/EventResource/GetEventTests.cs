using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class GetEventTests : BaseTest
{
    [Fact]
    public void EventResource_GetEvent_200()
    {
        var request = new GetEventRequest();
        var response = EventResourceApi.GetEvent(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public void EventResource_GetEvent_EmptyName_404()
    {
        var request = new GetEventByNameRequest() { ActiveOnly = true };
        var response = EventResourceApi.GetEvent(request, null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}