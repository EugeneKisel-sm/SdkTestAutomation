namespace SdkTestAutomation.Common.Comparison;

public interface IResultComparator
{
    Task<ComparisonResult> CompareAsync(string sdkOutput, string restApiResponse);
}

public class ComparisonResult
{
    public bool IsEqual { get; set; }
    public string Differences { get; set; } = string.Empty;
    public string SdkOutput { get; set; } = string.Empty;
    public string RestApiResponse { get; set; } = string.Empty;
} 