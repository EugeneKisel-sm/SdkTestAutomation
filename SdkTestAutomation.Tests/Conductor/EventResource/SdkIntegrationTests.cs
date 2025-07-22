using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Common.Models;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

public class SdkIntegrationTests : BaseTest
{
    [Fact]
    public async Task SdkIntegration_AddEvent_ValidatesAgainstApi()
    {
        // Arrange
        var eventName = $"test_event_sdk_{Guid.NewGuid():N}";
        var parameters = new Dictionary<string, object>
        {
            ["name"] = eventName,
            ["event"] = "test_event",
            ["active"] = true
        };

        // Act
        var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("add-event", parameters, "event");
        var apiResponse = EventResourceApi.AddEvent(new AddEventRequest
        {
            Name = eventName,
            Event = "test_event",
            Actions = new List<EventAction>(),
            Active = true
        });

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse), "SDK response does not match API response");
    }

    [Fact]
    public async Task SdkIntegration_GetEvent_ValidatesAgainstApi()
    {
        // Act
        var sdkResponse = await ExecuteSdkCallAsync<List<GetEventResponse>>("get-event", new Dictionary<string, object>(), "event");
        var apiResponse = EventResourceApi.GetEvent(new GetEventRequest());

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse), "SDK response does not match API response");
    }

    [Fact]
    public async Task SdkIntegration_GetEventByName_ValidatesAgainstApi()
    {
        // Arrange
        var eventName = "test_event";
        var parameters = new Dictionary<string, object>
        {
            ["event"] = eventName,
            ["activeOnly"] = true
        };

        // Act
        var sdkResponse = await ExecuteSdkCallAsync<List<GetEventResponse>>("get-event-by-name", parameters, "event");
        var apiResponse = EventResourceApi.GetEvent(new GetEventByNameRequest { Event = eventName, ActiveOnly = true }, eventName);

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse), "SDK response does not match API response");
    }

    [Fact]
    public async Task SdkIntegration_UpdateEvent_ValidatesAgainstApi()
    {
        // Arrange
        var eventName = $"test_event_update_{Guid.NewGuid():N}";
        
        // Create event first
        await ExecuteSdkCallAsync<GetEventResponse>("add-event", new Dictionary<string, object>
        {
            ["name"] = eventName,
            ["event"] = "test_event",
            ["active"] = true
        }, "event");

        // Act
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

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse), "SDK response does not match API response");
    }

    [Fact]
    public async Task SdkIntegration_DeleteEvent_ValidatesAgainstApi()
    {
        // Arrange
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
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse), "SDK response does not match API response");
    }
} 