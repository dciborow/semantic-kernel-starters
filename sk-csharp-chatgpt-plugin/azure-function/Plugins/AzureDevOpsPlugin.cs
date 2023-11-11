using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins.AzureDevOpsPlugin;

public class AzureDevOpsPlugin
{
    [SKFunction, Description("Get the work items linked to a build")]
    public static string BuildLinkedWorkItems(
      [Description("The build id of the Azure DevOps Build")] int buildId)
    {
        return GetBuildLinkedWorkItems(int buildId)
    }

    [SKFunction, Description("Get the pull requests linked to a build")]
    public static string BuildLinkedPullRequests(
      [Description("The build id of the Azure DevOps Build")] int buildId)
    {
        return GetBuildLinkedPullRequests(int buildId)
    }

    [SKFunction, Description("Get the commits linked to a build")]
    public static string BuildLinkedCommits(
      [Description("The build id of the Azure DevOps Build")] int buildId)
    {
        return GetBuildLinkedCommits(int buildId)
    }
}
