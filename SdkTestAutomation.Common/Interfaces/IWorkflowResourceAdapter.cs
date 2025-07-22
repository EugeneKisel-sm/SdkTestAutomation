using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Interfaces;

/// <summary>
/// Interface for workflow resource operations across different SDKs
/// </summary>
public interface IWorkflowResourceAdapter : ISdkAdapter
{
    /// <summary>
    /// Get workflow execution status
    /// </summary>
    Task<SdkResponse<GetWorkflowResponse>> GetWorkflowAsync(GetWorkflowRequest request);
} 