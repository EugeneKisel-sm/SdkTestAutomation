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
    
    public override Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        try
        {
            var eventHandler = CreateEventHandler(request);
            _javaEngine.RegisterEventHandler(eventHandler);
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        try
        {
            var events = _javaEngine.GetEventHandlers("", false);
            var firstEvent = events.FirstOrDefault();
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromJava(firstEvent)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        try
        {
            var events = _javaEngine.GetEventHandlers(request.Event, request.ActiveOnly ?? false);
            var firstEvent = events.FirstOrDefault();
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromJava(firstEvent)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        try
        {
            var eventHandler = CreateEventHandler(request);
            _javaEngine.UpdateEventHandler(eventHandler);
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        try
        {
            _javaEngine.UnregisterEventHandler(request.Name);
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse()));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
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