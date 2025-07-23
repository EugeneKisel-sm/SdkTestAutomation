using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class UpdateEventTests : BaseTest
{
    [Fact]
    public async Task EventResource_UpdateEvent_200()
    {
        /*var addRequest = new AddEventRequest
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
        Assert.False(response.Data.Active);*/

        {
            var eventName = $"test_event_update_{Guid.NewGuid():N}";

            // Create event first
            await ExecuteSdkCallAsync<GetEventResponse>("add-event", new Dictionary<string, object>
            {
                ["name"] = eventName,
                ["event"] = "test_event",
                ["active"] = true
            }, "event");

            var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("update-event", new Dictionary<string, object>
            {
                ["name"] = eventName,
                ["event"] = "test_event",
                ["active"] = false
            }, "event");

            var apiResponse = EventResourceApi.UpdateEvent(new AddEventRequest
            {
                Name = eventName,
                Event = "test_event",
                Actions = new List<EventAction>(),
                Active = false
            });

            Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
                "SDK response does not match API response");
        }
    }
}