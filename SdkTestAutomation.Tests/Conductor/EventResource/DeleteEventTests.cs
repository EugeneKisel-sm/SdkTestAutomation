using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class DeleteEventTests : BaseTest
{
    [Fact]
    public async Task EventResource_DeleteEvent_200()
    {
        /*var addRequest = new AddEventRequest
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
        Assert.DoesNotContain(getResponse.Data, e => e.Name == "test_event_delete");*/

        var eventName = $"test_event_delete_{Guid.NewGuid():N}";

        // Create event first
        await ExecuteSdkCallAsync<GetEventResponse>("add-event", new Dictionary<string, object>
        {
            ["name"] = eventName,
            ["event"] = "test_event",
            ["active"] = true
        }, "event");

        // Act
        var sdkResponse = await ExecuteSdkCallAsync<object>("delete-event", new Dictionary<string, object>
        {
            ["name"] = eventName
        }, "event");

        var apiResponse = EventResourceApi.DeleteEvent(new DeleteEventRequest(), eventName);

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
            "SDK response does not match API response");
    }
}