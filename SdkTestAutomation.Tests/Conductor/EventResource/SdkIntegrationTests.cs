using System.Net;
using SdkTestAutomation.Common.Models;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.EventResource;

/// <summary>
/// Integration tests for Conductor Event Resource using SDK adapters
/// </summary>
public class SdkIntegrationTests : BaseTest
{
    [Fact]
    public async Task SdkIntegration_AddEvent_ValidatesAgainstApi()
    {
        // Arrange
        var eventName = $"test_event_sdk_{Guid.NewGuid():N}";
        var request = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event",
            Actions = new List<EventAction>(),
            Active = true
        };

        // Act - Call SDK via adapter
        var eventAdapter = await GetEventResourceAdapterAsync();
        var sdkResponse = await eventAdapter.AddEventAsync(request);

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
        
        // Log adapter information
        LogAdapterInfo();
    }
    
    [Fact]
    public async Task SdkIntegration_GetEvent_ValidatesAgainstApi()
    {
        // Arrange
        var request = new GetEventRequest();

        // Act - Call SDK via adapter
        var eventAdapter = await GetEventResourceAdapterAsync();
        var sdkResponse = await eventAdapter.GetEventAsync(request);

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
        
        // Log adapter information
        LogAdapterInfo();
    }
} 