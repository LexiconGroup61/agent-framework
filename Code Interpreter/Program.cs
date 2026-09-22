using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;
using OpenAI.Chat;

DotNetEnv.Env.Load();
#pragma warning disable OPENAI001
var agent = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetResponsesClient()
#pragma warning restore OPENAI001
    .AsAIAgent(
        model: "gpt-6-astra",
        instructions: "You will receive coordinates from the user. Create a function that can calculate the distance between them and return the result.",
        tools: [new HostedCodeInterpreterTool()]
    );

var response = await agent.RunAsync("Latitude: 40.741895 / Longitude: 60.741895, Latitude: 37.741895 / Longitude: 56.741895");

Console.WriteLine(response.Text);

// Inspect code interpreter output from the response
foreach (var message in response.Messages)
{
    foreach (var content in message.Contents)
    {
        if (content is CodeInterpreterToolResultContent codeContent)
        {
            Console.WriteLine($"Code:\n{codeContent.Outputs[0].RawRepresentation}");
        }
    }
}