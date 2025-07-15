using System.Reflection;
using SdkTestAutomation.Core.Attributes;
using SdkTestAutomation.Utils.Extensions;
using SdkTestAutomation.Utils.Utilities;

namespace SdkTestAutomation.Core.Resolvers.Parameters
{
    internal abstract class RequestParametersResolver : IRequestParametersResolver
    {
        protected HttpRequest _request;

        public RequestParametersResolver(HttpRequest request)
        {
            _request = request;
        }

        public abstract string RequestBodyToString();

        public Dictionary<string, string> GetUrlParameters()
        {
            return HttpRequestItemsToDictionary<UrlParameterAttribute>();
        }

        public Dictionary<string, string> GetHeaders()
        {
            return HttpRequestItemsToDictionary<HeaderAttribute>();
        }

        protected Dictionary<string, string> HttpRequestItemsToDictionary<TReqItem>() where TReqItem : HttpRequestItemAttribute
        {
            return GetPropsValidatedAgainstNullCase()
                .Where(AttributeHelper.AttributeIsApplied<TReqItem>)
                .ToDictionary(GetStringNameOfProp, GetStringValueOfProp);
        }

        protected IEnumerable<PropertyInfo> GetPropsValidatedAgainstNullCase()
        {
            return _request.GetType()
                .GetProperties()
                .Where(prop => AttributeHelper.GetAttribute<HttpRequestItemAttribute>(prop) != null)
                .Where(prop => !AttributeHelper.GetAttribute<HttpRequestItemAttribute>(prop).IgnoreNullValue || prop.GetValue(_request) != null);
        }

        protected string GetStringValueOfProp(PropertyInfo prop)
        {
            return prop.PropertyType == typeof(DateTime) ? ((DateTime)prop.GetValue(_request)!).ToUtcString().Replace("Z", "") : prop.GetValue(_request)?.ToString();
        }

        protected string GetStringNameOfProp(PropertyInfo prop)
        {
            return AttributeHelper.GetAttribute<HttpRequestItemAttribute>(prop).Name
                    ?? prop.Name;
        }
    }
}
