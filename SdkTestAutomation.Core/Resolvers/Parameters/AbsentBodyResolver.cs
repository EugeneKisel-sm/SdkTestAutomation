namespace SdkTestAutomation.Core.Resolvers.Parameters
{
    internal class AbsentBodyResolver : RequestParametersResolver
    {
        public AbsentBodyResolver(HttpRequest request) : base(request)
        {

        }

        public override string RequestBodyToString()
        {
            return null;
        }
    }
}
