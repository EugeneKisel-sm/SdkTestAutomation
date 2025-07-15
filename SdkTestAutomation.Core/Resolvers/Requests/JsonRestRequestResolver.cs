using RestSharp;

namespace SdkTestAutomation.Core.Resolvers.Requests
{
    internal class JsonRestRequestResolver : RestRequestResolver
    {
        public JsonRestRequestResolver(HttpRequest request) : base(request)
        {
        }

        public override void AssignBody(IRestClient client, RestRequest restRequest)
        {
            var contentType = GetContentTypeHeader() ?? "application/json";
            var body = _request.GetBody();
            restRequest.RequestFormat = DataFormat.Json;
            if (body != null) restRequest.AddStringBody(body, contentType);
        }
    }
}
