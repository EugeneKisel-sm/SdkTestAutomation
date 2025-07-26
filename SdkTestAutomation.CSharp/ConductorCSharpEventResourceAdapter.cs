using Conductor.Client;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Adapters;
using EventHandler = Conductor.Client.Models.EventHandler;

namespace SdkTestAutomation.CSharp;

public class ConductorCSharpEventResourceAdapter : BaseEventResourceAdapter
{
    private CSharpConductorClient CSharpClient => (CSharpConductorClient)Client;
    public override string SdkType => "csharp";
    
    protected override ConductorClient CreateClient(string serverUrl) => new CSharpConductorClient(serverUrl);
    
    public override SdkResponse<GetEventResponse> AddEvent(AddEventRequest request)
    {
        try
        {
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            CSharpClient.EventApi.AddEventHandler(eventHandler);
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
            var events = CSharpClient.EventApi.GetEventHandlers();
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventMapper.MapFromCSharp(firstEvent));
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
            var events = CSharpClient.EventApi.GetEventHandlersForEvent(request.Event, request.ActiveOnly);
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventMapper.MapFromCSharp(firstEvent));
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
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            CSharpClient.EventApi.UpdateEventHandler(eventHandler);
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
            CSharpClient.EventApi.RemoveEventHandlerStatus(request.Name);
            return SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse());
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => SdkVersionHelper.GetAssemblyVersion(typeof(Configuration));
} 