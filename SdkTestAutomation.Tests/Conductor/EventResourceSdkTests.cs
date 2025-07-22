using System.Net;
using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Common.Configuration;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public class EventResourceSdkTests : BaseSdkTest
{
    [Fact]
    [Trait("Category", "SdkComparison")]
    public async Task GetEvent_CompareSdkWithRestApi_ShouldMatch()
    {
        var request = new GetEventRequest();
        
        await ValidateSdkResponseAsync(
            sdkCommand: "event list",
            sdkArgs: "",
            restApiCall: async () => EventResourceApi.GetEvent(request),
            testDescription: "Get all events comparison test");
    }
    
    [Fact]
    [Trait("Category", "SdkComparison")]
    public async Task AddEvent_CompareSdkWithRestApi_ShouldMatch()
    {
        var eventName = $"test_event_sdk_{Guid.NewGuid():N}";
        
        var request = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event_sdk",
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
        
        var sdkCommand = CurrentSdkType switch
        {
            SdkType.CSharp => $"event add --name \"{eventName}\" --event \"test_event_sdk\" --active true --action complete_task --expand-inline-json false",
            SdkType.Java => $"event add --name \"{eventName}\" --event \"test_event_sdk\" --active true --action complete_task --expandInlineJson false",
            SdkType.Python => $"event add --name \"{eventName}\" --event \"test_event_sdk\" --active true --action complete_task --expand_inline_json false",
            _ => throw new ArgumentException($"Unsupported SDK type: {CurrentSdkType}")
        };
        
        await ValidateSdkResponseAsync(
            sdkCommand: sdkCommand,
            sdkArgs: "",
            restApiCall: async () => EventResourceApi.AddEvent(request),
            testDescription: $"Add event '{eventName}' comparison test");
    }
    
    [Fact]
    [Trait("Category", "SdkComparison")]
    public async Task AddEventWithComplexActions_CompareSdkWithRestApi_ShouldMatch()
    {
        var eventName = $"test_event_complex_{Guid.NewGuid():N}";
        
        var request = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event_complex",
            Actions = new List<EventAction>
            {
                new()
                {
                    Action = "start_workflow",
                    StartWorkflow = new StartWorkflow
                    {
                        Name = "test_workflow",
                        Version = 1,
                        Input = new Dictionary<string, object>
                        {
                            ["key"] = "value"
                        }
                    },
                    ExpandInlineJson = true,
                }
            },
            Active = true
        };
        
        var sdkCommand = CurrentSdkType switch
        {
            SdkType.CSharp => $"event add --name \"{eventName}\" --event \"test_event_complex\" --active true --action start_workflow --start-workflow {{\\\"name\\\":\\\"test_workflow\\\",\\\"version\\\":1,\\\"input\\\":{{\\\"key\\\":\\\"value\\\"}}}} --expand-inline-json true",
            SdkType.Java => $"event add --name \"{eventName}\" --event \"test_event_complex\" --active true --action start_workflow --startWorkflow {{\\\"name\\\":\\\"test_workflow\\\",\\\"version\\\":1,\\\"input\\\":{{\\\"key\\\":\\\"value\\\"}}}} --expandInlineJson true",
            SdkType.Python => $"event add --name \"{eventName}\" --event \"test_event_complex\" --active true --action start_workflow --start_workflow {{\\\"name\\\":\\\"test_workflow\\\",\\\"version\\\":1,\\\"input\\\":{{\\\"key\\\":\\\"value\\\"}}}} --expand_inline_json true",
            _ => throw new ArgumentException($"Unsupported SDK type: {CurrentSdkType}")
        };
        
        await ValidateSdkResponseAsync(
            sdkCommand: sdkCommand,
            sdkArgs: "",
            restApiCall: async () => EventResourceApi.AddEvent(request),
            testDescription: $"Add complex event '{eventName}' comparison test");
    }
    
    [Fact]
    [Trait("Category", "SdkComparison")]
    public async Task GetEventByName_CompareSdkWithRestApi_ShouldMatch()
    {
        // First create an event
        var eventName = $"test_event_get_{Guid.NewGuid():N}";
        var addRequest = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event_get",
            Actions = new List<EventAction>(),
            Active = true
        };
        
        var addResponse = EventResourceApi.AddEvent(addRequest);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        
        // Now test getting it by name
        var getRequest = new GetEventByNameRequest { ActiveOnly = true };
        
        var sdkCommand = CurrentSdkType switch
        {
            SdkType.CSharp => $"event get --name \"{eventName}\" --active-only true",
            SdkType.Java => $"event get --name \"{eventName}\" --activeOnly true",
            SdkType.Python => $"event get --name \"{eventName}\" --active_only true",
            _ => throw new ArgumentException($"Unsupported SDK type: {CurrentSdkType}")
        };
        
        await ValidateSdkResponseAsync(
            sdkCommand: sdkCommand,
            sdkArgs: "",
            restApiCall: async () => EventResourceApi.GetEvent(getRequest, eventName),
            testDescription: $"Get event by name '{eventName}' comparison test");
    }
    
    [Fact]
    [Trait("Category", "SdkComparison")]
    public async Task DeleteEvent_CompareSdkWithRestApi_ShouldMatch()
    {
        // First create an event
        var eventName = $"test_event_delete_{Guid.NewGuid():N}";
        var addRequest = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event_delete",
            Actions = new List<EventAction>(),
            Active = true
        };
        
        var addResponse = EventResourceApi.AddEvent(addRequest);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        
        // Now test deleting it
        var deleteRequest = new DeleteEventRequest();
        
        var sdkCommand = $"event delete --name \"{eventName}\"";
        
        await ValidateSdkResponseAsync(
            sdkCommand: sdkCommand,
            sdkArgs: "",
            restApiCall: async () => EventResourceApi.DeleteEvent(deleteRequest, eventName),
            testDescription: $"Delete event '{eventName}' comparison test");
    }
    
    [Fact]
    [Trait("Category", "SdkComparison")]
    public async Task UpdateEvent_CompareSdkWithRestApi_ShouldMatch()
    {
        // First create an event
        var eventName = $"test_event_update_{Guid.NewGuid():N}";
        var addRequest = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event_update",
            Actions = new List<EventAction>(),
            Active = true
        };
        
        var addResponse = EventResourceApi.AddEvent(addRequest);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        
        // Now test updating it
        var updateRequest = new AddEventRequest
        {
            Name = eventName,
            Event = "test_event_updated",
            Actions = new List<EventAction>(),
            Active = false
        };
        
        var sdkCommand = CurrentSdkType switch
        {
            SdkType.CSharp => $"event update --name \"{eventName}\" --event \"test_event_updated\" --active false",
            SdkType.Java => $"event update --name \"{eventName}\" --event \"test_event_updated\" --active false",
            SdkType.Python => $"event update --name \"{eventName}\" --event \"test_event_updated\" --active false",
            _ => throw new ArgumentException($"Unsupported SDK type: {CurrentSdkType}")
        };
        
        await ValidateSdkResponseAsync(
            sdkCommand: sdkCommand,
            sdkArgs: "",
            restApiCall: async () => EventResourceApi.UpdateEvent(updateRequest),
            testDescription: $"Update event '{eventName}' comparison test");
    }
} 