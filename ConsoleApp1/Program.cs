


using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

DotNetEnv.Env.Load();
Console.WriteLine(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"));
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetChatClient("gpt-5.4")
    .AsAIAgent(
        instructions: "You are a helpful assistant with ability to search the web for current basketball news.",
        tools: [new HostedWebSearchTool()]);
        
Console.WriteLine(await client.RunAsync("Why did LA Lakers sign Walker Kessler?"));