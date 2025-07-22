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
        var request = new AddEventRequest
        {
            Name = "test_event_add",
            Event = "test_event",
            Actions = new List<EventAction>
            {
            },
            Active = true
        };
        
        var response = EventResourceApi.AddEvent(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}