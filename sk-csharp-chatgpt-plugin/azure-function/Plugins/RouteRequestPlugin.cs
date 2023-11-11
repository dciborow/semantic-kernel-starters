using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins.RouteRequestPlugin;

public class RouteRequestPlugin
{
    [SKFunction, Description("Routes the request to the appropriate function.")]
    public async Task<string> RouteRequestAsync(
        [Description("The user request")] string input
    )
    {
        // Save the original user request
        string request = input;
    
        // Retrieve the intent from the user request
        var getIntent = _kernel.Functions.GetFunction("OrchestratorPlugin", "GetIntent");
        var getIntentVariables = new ContextVariables
        {
            ["input"] = input,
            ["options"] = "Board, Code, PullRequest, Build, Release"
        };
        string intent = (await _kernel.RunAsync(getIntentVariables, getIntent)).GetValue<string>()!.Trim();
    
        // Call the appropriate function
        IAzureDevOpsPlugin AzureDevOpsPlugin;
        switch (intent)
        {
            case "Board":
                AzureDevOpsPlugin =  _kernel.ImportFunctions(new Plugins.AzureDevOpsPlugin.AzureDevOpsReleasePlugin(), "AzureDevOpsBoardPlugin");
                break;
            case "Code":
                AzureDevOpsPlugin =  _kernel.ImportFunctions(new Plugins.AzureDevOpsPlugin.AzureDevOpsReleasePlugin(), "AzureDevOpsCodePlugin");
                break;
            case "PullRequest":
                AzureDevOpsPlugin =  _kernel.ImportFunctions(new Plugins.AzureDevOpsPlugin.AzureDevOpsReleasePlugin(), "AzureDevOpsPullRequestPlugin");
                break;
            case "Build":
                AzureDevOpsPlugin = _kernel.ImportFunctions(new Plugins.AzureDevOpsPlugin.AzureDevOpsBuildPlugin(), "AzureDevOpsBuildPlugin");
                break;
            case "Release":
                AzureDevOpsPlugin =  _kernel.ImportFunctions(new Plugins.AzureDevOpsPlugin.AzureDevOpsReleasePlugin(), "AzureDevOpsReleasePlugin");
                break;
            default:
                AzureDevOpsPlugin = _kernel.ImportFunctions(new Plugins.CommonPlugin.Common(), "GenerateNewPlugin");
        }
        
        // Use Plugin to Load Optimal Plan if it it exists or create a new one.
        LoadPlanPlugin =  _kernel.ImportFunctions(new Plugins.CommonPlugin.Common(), "LoadPlanPlugin");

        // Create planner
        var planner = new SequentialPlanner(_kernel);
        var plan = await planner.CreatePlanAsync(request);
        // Run the pipeline
        var output = await this._kernel.RunAsync(plan);

        Console.WriteLine("Plan:\n");
        Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine("Plan results:");
        Console.WriteLine(output.GetValue<string>()!.Trim());

        return output.GetValue<string>()!;
    }
}
