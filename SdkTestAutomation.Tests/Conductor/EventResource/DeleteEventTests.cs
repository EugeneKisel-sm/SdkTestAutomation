using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class DeleteEventTests : BaseTest
{
    [Fact]
    public void EventResource_DeleteEvent_200()
    {
        var addRequest = new AddEventRequest
        {
            Name = "test_event_delete",
            Event = "test_event_d",
            Actions = new List<EventAction>
            {
                new()
                {
                    Action = "complete_task",
                    ExpandInlineJson = false,
                }
            },
            Active = true
        };
        
        var addResponse = EventResourceApi.AddEvent(addRequest);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        Assert.True(addResponse.Data.Active);
        
        var deleteRequest = new DeleteEventRequest();
        var response = EventResourceApi.DeleteEvent(deleteRequest, "test_event_delete");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var getRequest = new GetEventByNameRequest();
        var getResponse = EventResourceApi.GetEvent(getRequest, "test_event_delete");
        Assert.DoesNotContain(getResponse.Data, e => e.Name == "test_event_delete");
    }
}