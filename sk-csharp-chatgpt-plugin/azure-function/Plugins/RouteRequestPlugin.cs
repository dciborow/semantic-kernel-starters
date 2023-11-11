using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins.RouteRequestPlugin;

public class RouteRequestPlugin
{
    [SKFunction, Description("Extracts numbers from JSON")]
    public static SKContext ExtractNumbersFromJson(SKContext context)
    {
        JObject numbers = JObject.Parse(context.Variables["input"]);
    
        // Loop through numbers and add them to the context
        foreach (var number in numbers)
        {
            if (number.Key == "number1")
            {
                context.Variables["input"] = number.Value!.ToString();
                continue;
            }
    
            context.Variables[number.Key] = number.Value!.ToString();
        }
        return context;
    }

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
            ["options"] = "Sqrt, Multiply"
        };
        string intent = (await _kernel.RunAsync(getIntentVariables, getIntent)).GetValue<string>()!.Trim();
    
        // Call the appropriate function
        ISKFunction MathFunction;
        switch (intent)
        {
            case "Sqrt":
                MathFunction = this._kernel.Functions.GetFunction("MathPlugin", "Sqrt");
                break;
            case "Multiply":
                MathFunction = this._kernel.Functions.GetFunction("MathPlugin", "Multiply");
                break;
            default:
                return "I'm sorry, I don't understand.";
        }
        
        // Create planner
        var planner = new SequentialPlanner(_kernel);
        var ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?";
        var plan = await planner.CreatePlanAsync(ask);
        
        Console.WriteLine("Plan:\n");
        Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

        // Get remaining functions
        var createResponse = this._kernel.Functions.GetFunction("OrchestratorPlugin", "CreateResponse");
        var getNumbers = this._kernel.Functions.GetFunction("OrchestratorPlugin", "GetNumbers");
        var extractNumbersFromJson = this._kernel.Functions.GetFunction("OrchestratorPlugin", "ExtractNumbersFromJson");

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
