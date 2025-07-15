using RestSharp;

namespace SdkTestAutomation.Core.Resolvers.Requests
{
    internal class FormUrlEncodedRestRequestResolver : RestRequestResolver
    {
        public FormUrlEncodedRestRequestResolver(HttpRequest request) : base(request)
        {
        }

        public override void AssignBody(IRestClient client, RestRequest restRequest)
        {
            var contentType = GetContentTypeHeader() ?? "application/x-www-form-urlencoded";
            var body = _request.GetBody();
            if(body != null) restRequest.AddStringBody(body, contentType);
        }
    }
}
