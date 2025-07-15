using SdkTestAutomation.Core.Resolvers.Parameters;
using SdkTestAutomation.Core.Resolvers.Requests;

namespace SdkTestAutomation.Core;

internal static class RequestResolvers
{
    internal static IRestRequestResolver GetRestRequestResolver(HttpRequest request)
    {
        return request.BodyContentType switch
        {
            ContentType.Json => new JsonRestRequestResolver(request),
            ContentType.FormUrlEncoded => new FormUrlEncodedRestRequestResolver(request),
            ContentType.None => new AbsentBodyRestRequestResolver(request),
            _ => throw new NotImplementedException(
                $"{nameof(request.BodyContentType)} was not recognized: {request.BodyContentType}")
        };
    }

    internal static IRequestParametersResolver GetRequestParametersResolver(HttpRequest request)
    {
        return request.BodyContentType switch
        {
            ContentType.Json => new JsonRequestParametersResolver(request),
            ContentType.FormUrlEncoded => new FormUrlEncodedRequestParametersResolver(request),
            ContentType.None => new AbsentBodyResolver(request),
            _ => throw new NotImplementedException(
                $"{nameof(request.BodyContentType)} was not recognized: {request.BodyContentType}")
        };
    }
}