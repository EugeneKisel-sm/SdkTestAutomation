using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Core.Interfaces;

public interface IEventAdapter : ISdkAdapter
{
    SdkResponse AddEvent(string name, string eventType, bool active = true);
    SdkResponse GetEvents();
    SdkResponse GetEventByName(string eventName);
    SdkResponse UpdateEvent(string name, string eventType, bool active = true);
    SdkResponse DeleteEvent(string name);
} 