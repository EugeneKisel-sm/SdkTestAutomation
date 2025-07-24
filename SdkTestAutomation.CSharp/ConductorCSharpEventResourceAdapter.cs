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
    private EventResourceApi _eventApi;
    
    public override string SdkType => "csharp";
    
    public override Task<bool> InitializeAsync(AdapterConfiguration config)
    {
        try
        {
            _config = config;
            LogOperation("Initializing C# SDK adapter", config.ServerUrl);
            
            var configuration = new Configuration { BasePath = config.ServerUrl };
            _eventApi = new EventResourceApi(configuration);
            
            LogOperation("C# SDK adapter initialized successfully");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            LogError("initializing C# SDK adapter", ex);
            return Task.FromResult(false);
        }
    }
    
    public override Task<bool> IsHealthyAsync()
    {
        try
        {
            if (_eventApi == null) return Task.FromResult(false);
            
            // Try to get events to check if the API is accessible
            return Task.Run(() => 
            {
                try
                {
                    _eventApi.GetEventHandlers();
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Adding event", request.Name);
            
            // Create a basic EventHandler with only known properties
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            await Task.Run(() => _eventApi!.AddEventHandler(eventHandler));
            return SdkResponseBuilder.CreateFromRequest(request);
        }
        catch (ApiException ex)
        {
            LogError("adding event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            LogError("adding event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Getting all events");
            
            var events = await Task.Run(() => _eventApi!.GetEventHandlers());
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapCSharpCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data);
        }
        catch (ApiException ex)
        {
            LogError("getting events", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            LogError("getting events", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Getting events by name", request.Event);
            
            var events = await Task.Run(() => _eventApi!.GetEventHandlersForEvent(request.Event, request.ActiveOnly));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapCSharpCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data);
        }
        catch (ApiException ex)
        {
            LogError("getting events by name", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            LogError("getting events by name", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Updating event", request.Name);
            
            // Create a basic EventHandler with only known properties
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            await Task.Run(() => _eventApi!.UpdateEventHandler(eventHandler));
            return SdkResponseBuilder.CreateFromRequest(request);
        }
        catch (ApiException ex)
        {
            LogError("updating event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            LogError("updating event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Deleting event", request.Name);
            
            await Task.Run(() => _eventApi!.RemoveEventHandlerStatus(request.Name));
            return SdkResponseBuilder.CreateEmptyResponse();
        }
        catch (ApiException ex)
        {
            LogError("deleting event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            LogError("deleting event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => "1.1.3";
    
    protected override bool IsInitialized() => _eventApi != null;
    
    public override void Dispose()
    {
        _eventApi = null;
    }
} 