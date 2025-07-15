using Newtonsoft.Json;
using RestSharp;
using SdkTestAutomation.Utils.Extensions;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Core
{
    public class HttpClient
    {
        public RestClient Client;
        private readonly ILogger logger;

        public HttpClient(ILogger logger, int timeout = 80000)
        {
            var clientOptions = new RestClientOptions()
            {
                Timeout = TimeSpan.FromMilliseconds(timeout)
            };
            Client = new RestClient(clientOptions);
            this.logger = logger;
        }

        public virtual RestResponse<T> SendGetRequest<T>(string url, HttpRequest requestModel)
        {
            return SendRequest<T>(url, Method.Get, requestModel);
        }

        public virtual RestResponse<T> SendPostRequest<T>(string url, HttpRequest requestModel)
        {
            return SendRequest<T>(url, Method.Post, requestModel);
        }

        public virtual RestResponse<T> SendPutRequest<T>(string url, HttpRequest requestModel)
        {
            return SendRequest<T>(url, Method.Put, requestModel);
        }

        public virtual RestResponse<T> SendDeleteRequest<T>(string url, HttpRequest requestModel)
        {
            return SendRequest<T>(url, Method.Delete, requestModel);
        }

        public virtual RestResponse<T> SendPatchRequest<T>(string url, HttpRequest requestModel)
        {
            return SendRequest<T>(url, Method.Patch, requestModel);
        }

        public virtual RestResponse<T> SendRequest<T>(string url, Method method, HttpRequest requestModel)
        {
            var request = new RestRequest(url, method);
            var resolver = RequestResolvers.GetRestRequestResolver(requestModel);
            resolver.AssignUrlParameters(request);
            resolver.AssignHeaders(request);
            resolver.AssignBody(Client, request);

            Client.BuildUri(request);
            logger.Log(DateTime.UtcNow.ToUtcString());
            logger.Log("Sending request:");
            logger.Log($"{method} {request.Resource}");
            logger.Log(requestModel.ToString());

            var response = Client.Execute<T>(request);
            if (typeof(T) == typeof(byte[]))
            {
                response.Data = (T)(object)Client.DownloadData(request);
            }

            logger.Log(DateTime.UtcNow.ToUtcString());
            logger.Log("Response:");
            logger.Log($"{(int)response.StatusCode} {response.StatusCode}");

            if (response.Content != null)
            {
                logger.Log($"Body:\n{response.Content}\n");
            }

            if (response.ErrorMessage != null)
            {
                logger.Log($"Error:\n{response.ErrorMessage}\n");
                return response;
            }

            return HandleResponse(response);
        }

        private RestResponse<T> HandleResponse<T>(RestResponse<T> response)
        {
            if (response?.ContentType == null)
			{
				response.Data = default;
				return response;
			}

            if (response.ContentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (JsonSerializationException ex)
                {
                    logger.Log($"JSON deserialization error: {ex.Message}");
                    response.Data = default;
                }
            }

            return response;
        }
    }
}

