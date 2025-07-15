using Newtonsoft.Json;
using System.Text;

namespace SdkTestAutomation.Core
{
    public abstract class HttpRequest
    {
        [JsonIgnore] 
        private string _content;

        [JsonIgnore] 
        public ContentType BodyContentType { get; set; } = ContentType.Json;

        [JsonIgnore]
        public virtual string Content
        {
            get => GetBody();
            set => _content = value;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            var parameters = GetUrlParameters();
            if (parameters?.Count > 0)
            {
                builder.AppendLine("Parameters");
                builder.AppendLine(DictionaryToJson(parameters));
            }

            var headers = GetHeaders();
            if (headers?.Count > 0)
            {
                if (builder.Length > 0) builder.AppendLine();
                builder.AppendLine("Headers");
                builder.AppendLine(DictionaryToJson(headers));
            }

            var body = GetBody();
            if (!string.IsNullOrEmpty(body))
            {
                if (builder.Length > 0) builder.AppendLine();
                builder.AppendLine("Body");
                builder.AppendLine(body);
            }

            return builder.ToString();
        }

        protected internal virtual Dictionary<string, string> GetUrlParameters()
        {
            return RequestResolvers.GetRequestParametersResolver(this).GetUrlParameters();
        }

        protected internal virtual Dictionary<string, string> GetHeaders()
        {
            return RequestResolvers.GetRequestParametersResolver(this).GetHeaders();
        }

        protected internal virtual string GetBody()
        {
            return _content ?? RequestResolvers.GetRequestParametersResolver(this).RequestBodyToString();
        }

        private string DictionaryToJson(Dictionary<string, string> dict)
        {
            return JsonConvert.SerializeObject(dict);
        }
    }
}

