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
        // var sampleRequest = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?";
        var request = input;
        var mathPlugin = kernel.ImportFunctions(new Plugins.AzureDevOpsPlugin.AzureDevOps(), "AzureDevOpsPlugin");

        // Create planner
        var planner = new SequentialPlanner(_kernel);
        var plan = await planner.CreatePlanAsync(ask);
        
        Console.WriteLine("Plan:\n");
        Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

        // Run the pipeline
        var output = await this._kernel.RunAsync(
            request,
            getNumbers,
            extractNumbersFromJson,
            MathFunction,
            createResponse
        );
        Console.WriteLine("Plan results:");
        Console.WriteLine(output.GetValue<string>()!.Trim());

        return output.GetValue<string>()!;
    }
}
