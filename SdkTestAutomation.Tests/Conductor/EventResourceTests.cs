using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public class EventResourceTests : BaseTest
{
    [Fact]
    public void EventResources_GetEvent_200()
    {
        var request = new GetEventRequest();
        var response = EventResourceApi.GetEvent(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public void EventResources_GetEvent_EmptyName_404()
    {
        var request = new GetEventByNameRequest() { ActiveOnly = true };
        var response = EventResourceApi.GetEvent(request, null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public void AddEventHandler_ShouldCreateNewEventHandler()
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
    
    [Fact]
    public void UpdateEventHandler_ShouldModifyExistingEventHandler()
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
    
    [Fact]
    public void DeleteEventHandler_ShouldRemoveEventHandler()
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