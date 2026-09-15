

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
        model: "gpt-5.4",
        instructions: "You are a skilled commentator on sports with the ability to search the web for current news",
        tools: [new HostedWebSearchTool()]
        );

var response = await agent.RunAsync("Why did LA Lakers trade for Walker Kessler");

Console.WriteLine(response.Text);