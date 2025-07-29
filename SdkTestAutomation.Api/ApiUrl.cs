using SdkTestAutomation.Utils;

namespace SdkTestAutomation.Api
{
    public static class ApiUrl
    {
        public static class EventResource
        {
            public static string EventUrl => $"{TestConfig.ApiUrl}/event";
        }
        
        public static class TokenResource
        {
            public static string TokenUrl => $"{TestConfig.ApiUrl}/token";
        }
        
        public static class WorkflowResource
        {
            public static string WorkflowUrl => $"{TestConfig.ApiUrl}/workflow";
        }
    }
}