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
            var bodyProps = HttpRequestItemsToDictionary<BodyAttribute>();
            var bodyPropNames = bodyProps.Select(pair => pair.Key).ToArray();

            if (bodyPropNames.Length == 0)
            {
                return null;
            }

            // Get the JSON property names that correspond to the C# property names
            var jsonPropertyNames = new List<string>();
            foreach (var propName in bodyPropNames)
            {
                var property = _request.GetType().GetProperty(propName);
                if (property != null)
                {
                    var jsonPropertyAttr = property.GetCustomAttributes(typeof(JsonPropertyAttribute), false).FirstOrDefault() as JsonPropertyAttribute;
                    var jsonName = jsonPropertyAttr?.PropertyName ?? propName;
                    jsonPropertyNames.Add(jsonName);
                }
            }

            return JsonConvert.SerializeObject(_request, Formatting.None, 
                new JsonSerializerSettings { ContractResolver = new DynamicContractResolver(_request.GetType(), jsonPropertyNames.ToArray()) });
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
                    // Check if this property should be included based on the target property names
                    bool shouldInclude = false;
                    
                    // Check if the property name matches any target property
                    if (_targetProps.Contains(property.PropertyName))
                    {
                        shouldInclude = true;
                    }
                    else
                    {
                        // Check if the assigned name (from HttpRequestItemAttribute) matches any target property
                        var assignedName = HttpRequestItemAttribute.ConvertRealPropNameToAssigned(type, property.PropertyName);
                        if (_targetProps.Contains(assignedName))
                        {
                            shouldInclude = true;
                        }
                    }
                    
                    if (shouldInclude)
                    {
                        filteredProperties.Add(property);
                    }
                }

                return filteredProperties;
            }
        }
    }
}
