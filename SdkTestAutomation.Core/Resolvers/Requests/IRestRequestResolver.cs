using RestSharp;

namespace SdkTestAutomation.Core.Resolvers.Requests
{
    internal interface IRestRequestResolver
    {
        void AssignUrlParameters(RestRequest restRequest);
        void AssignHeaders(RestRequest restRequest);
        void AssignBody(IRestClient client, RestRequest restRequest);
    }
}
