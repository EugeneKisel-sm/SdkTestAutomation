using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class UpdateEventTests : BaseTest
{
    [Fact]
    public void EventResource_UpdateEvent_200()
    {
        var addRequest = new AddEventRequest
        {
            Name = "test_event_update",
            Event = "test_event_u",
            Actions = new List<EventAction>
            {
                new()
                {
                    Action = "complete_task",
                    ExpandInlineJson = false,
                    StartWorkflow = new StartWorkflow()
                }
            },
            Active = true
        };
        
        var addResponse = EventResourceApi.AddEvent(addRequest);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        Assert.True(addResponse.Data.Active);
        
        var updateRequest = new AddEventRequest()
        {
            Name = "test_event_handler_update",
            Event = "test_event",
            Active = false
        };
        
        var response = EventResourceApi.UpdateEvent(updateRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(response.Data.Active);
    }
}