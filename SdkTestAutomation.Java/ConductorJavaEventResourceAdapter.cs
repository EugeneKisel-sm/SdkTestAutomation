using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Sdk.Helpers;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Java.JavaBridge;

namespace SdkTestAutomation.Java;

/// <summary>
/// Java SDK adapter for event resource operations using IKVM.NET
/// </summary>
public class ConductorJavaEventResourceAdapter : BaseEventResourceAdapter
{
    private JavaEngine _javaEngine;
    
    public override string SdkType => "java";
    
    protected override Task InitializeEngineAsync()
    {
        _javaEngine = new JavaEngine();
        _javaEngine.Initialize(Config);
        return Task.CompletedTask;
    }
    
    protected override void PerformHealthCheck()
    {
        _javaEngine.GetEventHandlers("", false);
    }
    
    public override SdkResponse<GetEventResponse> AddEvent(AddEventRequest request)
    {
        try
        {
            var eventHandler = CreateEventHandler(request);
            _javaEngine.RegisterEventHandler(eventHandler);
            return SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> GetEvent(GetEventRequest request)
    {
        try
        {
            var events = _javaEngine.GetEventHandlers("", false);
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromJava(firstEvent));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> GetEventByName(GetEventByNameRequest request)
    {
        try
        {
            var events = _javaEngine.GetEventHandlers(request.Event, request.ActiveOnly ?? false);
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromJava(firstEvent));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> UpdateEvent(UpdateEventRequest request)
    {
        try
        {
            var eventHandler = CreateEventHandler(request);
            _javaEngine.UpdateEventHandler(eventHandler);
            return SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> DeleteEvent(DeleteEventRequest request)
    {
        try
        {
            _javaEngine.UnregisterEventHandler(request.Name);
            return SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse());
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    private dynamic CreateEventHandler(dynamic request)
    {
        var eventHandler = _javaEngine.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler");
        eventHandler.setName(request.Name);
        eventHandler.setEvent(request.Event);
        eventHandler.setActive(request.Active);
        return eventHandler;
    }
    
    protected override string GetSdkVersion() => "3.15.0";
    
    protected override bool IsInitialized() => _javaEngine != null;
    
    protected override void DisposeEngine()
    {
        _javaEngine = null;
    }
} 