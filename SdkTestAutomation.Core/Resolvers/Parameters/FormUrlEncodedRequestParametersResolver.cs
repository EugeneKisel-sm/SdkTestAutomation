using System.Web;
using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Core.Resolvers.Parameters
{
    internal class FormUrlEncodedRequestParametersResolver : RequestParametersResolver
    {
        public FormUrlEncodedRequestParametersResolver(HttpRequest request) : base(request)
        {

        }

        public override string RequestBodyToString()
        {
            var parameters = HttpRequestItemsToDictionary<BodyAttribute>()
                .Select(pair => $"{pair.Key}={HttpUtility.UrlEncode(pair.Value)}");
            return string.Join("&", parameters);
        }
    }
}
