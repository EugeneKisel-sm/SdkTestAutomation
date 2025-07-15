using SdkTestAutomation.Utils.Utilities;

namespace SdkTestAutomation.Core.Attributes
{
    public abstract class HttpRequestItemAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IgnoreNullValue { get; protected init; } = true;
        
        internal static string ConvertRealPropNameToAssigned(Type targetType, string realPropName)
        {
            var attr = AttributeHelper.GetAttribute<HttpRequestItemAttribute>(targetType.GetProperty(realPropName));
            return attr?.Name ?? realPropName;
        }
    }
}
