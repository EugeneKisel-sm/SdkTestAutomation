using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Core.Resolvers.Parameters
{
    internal class JsonRequestParametersResolver : RequestParametersResolver
    {
        public JsonRequestParametersResolver(HttpRequest request) : base(request)
        {
        }

        public override string RequestBodyToString()
        {
            var bodyPropNames = HttpRequestItemsToDictionary<BodyAttribute>()
                .Select(pair => pair.Key)
                .ToArray();

            if (bodyPropNames.Length == 0)
            {
                return null;
            }

            return JsonConvert.SerializeObject(_request, Formatting.None, 
                new JsonSerializerSettings { ContractResolver = new DynamicContractResolver(_request.GetType(), bodyPropNames) });
        }

        private class DynamicContractResolver : DefaultContractResolver
        {
            private readonly HashSet<string> _targetProps;
            private readonly Type _typeToBeConfigured;

            public DynamicContractResolver(Type typeToBeConfigured, string[] targetProps)
            {
                _typeToBeConfigured = typeToBeConfigured ?? throw new ArgumentNullException(nameof(typeToBeConfigured));
                _targetProps = new HashSet<string>(targetProps ?? Array.Empty<string>());
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var allProperties = base.CreateProperties(type, memberSerialization);
                
                if (type != _typeToBeConfigured)
                {
                    return allProperties;
                }

                var filteredProperties = new List<JsonProperty>();
                foreach (var property in allProperties)
                {
                    if (_targetProps.Contains(property.PropertyName))
                        {
                        filteredProperties.Add(property);
                            continue;
                        }

                    var assignedName = HttpRequestItemAttribute.ConvertRealPropNameToAssigned(type, property.PropertyName);
                        if (_targetProps.Contains(assignedName))
                        {
                        property.PropertyName = assignedName;
                        filteredProperties.Add(property);
                        }
                    }

                return filteredProperties;
            }
        }
    }
}
