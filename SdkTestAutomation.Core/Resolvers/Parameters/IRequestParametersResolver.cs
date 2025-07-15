namespace SdkTestAutomation.Core.Resolvers.Parameters
{
    internal interface IRequestParametersResolver
    {
        string RequestBodyToString();
        Dictionary<string, string> GetHeaders();
        Dictionary<string, string> GetUrlParameters();
    }
}
