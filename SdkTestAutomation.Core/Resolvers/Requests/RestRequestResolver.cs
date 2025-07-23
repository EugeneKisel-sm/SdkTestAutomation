using RestSharp;

namespace SdkTestAutomation.Core.Resolvers.Requests
{
    internal abstract class RestRequestResolver : IRestRequestResolver
    {
        private const string ContentTypeHeaderKey = "content-type";
        protected readonly HttpRequest _request;

        public RestRequestResolver(HttpRequest request)
        {
            _request = request;
        }

        public abstract void AssignBody(IRestClient client, RestRequest restRequest);

        public void AssignHeaders(RestRequest restRequest)
        {
            foreach (var header in _request.GetHeaders())
            {
                restRequest.AddHeader(header.Key, header.Value);
            }
        }

        public void AssignUrlParameters(RestRequest restRequest)
        {
            foreach (var parameter in _request.GetUrlParameters())
            {
                restRequest.AddQueryParameter(parameter.Key, parameter.Value);
            }
        }

        protected string GetContentTypeHeader() => _request
            .GetHeaders()
            .FirstOrDefault(header => header.Key.Equals(ContentTypeHeaderKey, StringComparison.OrdinalIgnoreCase))
            .Value;
    }
}
