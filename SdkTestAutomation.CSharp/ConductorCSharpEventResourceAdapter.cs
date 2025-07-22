using Conductor.Api;
using Conductor.Client;
using Conductor.Client.Models;
using SdkTestAutomation.Common.Helpers;
using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using Task = System.Threading.Tasks.Task;
using EventHandler = Conductor.Client.Models.EventHandler;

namespace SdkTestAutomation.CSharp;

/// <summary>
/// C# SDK adapter for event resource operations
/// </summary>
public class ConductorCSharpEventResourceAdapter : BaseEventResourceAdapter
{
    private EventResourceApi? _eventApi;
    
    public override string SdkType => "csharp";
    
    public override async Task<bool> InitializeAsync(AdapterConfiguration config)
    {
        try
        {
            _config = config;
            LogOperation("Initializing C# SDK adapter", config.ServerUrl);
            
            var configuration = new Configuration { BasePath = config.ServerUrl };
            _eventApi = new EventResourceApi(configuration);
            
            LogOperation("C# SDK adapter initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            LogError("initializing C# SDK adapter", ex);
            return false;
        }
    }
    
    public override async Task<bool> IsHealthyAsync()
    {
        try
        {
            if (_eventApi == null) return false;
            
            // Try to get events to check if the API is accessible
            await Task.Run(() => _eventApi.GetEventHandlers());
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        return await ExecuteEventOperation("Adding event", request.Name, request.RequestId, async () =>
        {
            // Create a basic EventHandler with only known properties
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            await Task.Run(() => _eventApi!.AddEventHandler(eventHandler));
            return SdkResponseBuilder.CreateFromRequest(request);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        return await ExecuteEventOperation("Getting all events", null, request.RequestId, async () =>
        {
            var events = await Task.Run(() => _eventApi!.GetEventHandlers());
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapCSharpCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data, request.RequestId);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        return await ExecuteEventOperation("Getting events by name", request.Event, request.RequestId, async () =>
        {
            var events = await Task.Run(() => _eventApi!.GetEventHandlersForEvent(request.Event, request.ActiveOnly));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapCSharpCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data, request.RequestId);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        return await ExecuteEventOperation("Updating event", request.Name, request.RequestId, async () =>
        {
            // Create a basic EventHandler with only known properties
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            await Task.Run(() => _eventApi!.UpdateEventHandler(eventHandler));
            return SdkResponseBuilder.CreateFromRequest(request);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        return await ExecuteEventOperation("Deleting event", request.Name, request.RequestId, async () =>
        {
            await Task.Run(() => _eventApi!.RemoveEventHandlerStatus(request.Name));
            return SdkResponseBuilder.CreateEmptyResponse(request.RequestId);
        });
    }
    
    /// <summary>
    /// Execute event operation with common error handling
    /// </summary>
    private async Task<SdkResponse<GetEventResponse>> ExecuteEventOperation(
        string operation, 
        string? details, 
        string requestId, 
        Func<Task<SdkResponse<GetEventResponse>>> operationFunc)
    {
        try
        {
            ValidateInitialization();
            LogOperation(operation, details);
            
            return await operationFunc();
        }
        catch (ApiException ex)
        {
            LogError(operation.ToLower(), ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, requestId, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            LogError(operation.ToLower(), ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, requestId);
        }
    }
    
    protected override string GetSdkVersion() => "1.1.3";
    
    protected override bool IsInitialized() => _eventApi != null;
    
    public override void Dispose()
    {
        _eventApi = null;
    }
} 