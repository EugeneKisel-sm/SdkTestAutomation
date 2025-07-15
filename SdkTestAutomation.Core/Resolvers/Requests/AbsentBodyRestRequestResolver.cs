using RestSharp;

namespace SdkTestAutomation.Core.Resolvers.Requests
{
    internal class AbsentBodyRestRequestResolver : RestRequestResolver
    {
        public AbsentBodyRestRequestResolver(HttpRequest request) : base(request)
        {
        }

        public override void AssignBody(IRestClient client, RestRequest restRequest)
        {
        }
    }
}
