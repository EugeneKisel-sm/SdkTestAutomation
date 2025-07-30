using System.Reflection;

namespace SdkTestAutomation.Utils.Utilities
{
    public static class AttributeHelper
    {
        public static TAttribute GetAttribute<TAttribute>(MemberInfo memberInfo) where TAttribute : Attribute
        {
            return (TAttribute)memberInfo
                .GetCustomAttributes(typeof(TAttribute), false)
                .SingleOrDefault();
        }

        public static bool AttributeIsApplied<TAttribute>(MemberInfo memberInfo) where TAttribute : Attribute
        {
            return memberInfo
                .GetCustomAttributes(typeof(TAttribute), false)
                .Length != 0;
        }
    }
}