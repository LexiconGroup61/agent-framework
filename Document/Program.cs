
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

// // Setting up the AI agentChat

DotNetEnv.Env.Load();
Console.WriteLine(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"));
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetChatClient("GPT-4.1")
    .AsIChatClient();

// Reading the pdf

var byteData = await File.ReadAllBytesAsync("ddd.pdf");
var readOnlyData = new ReadOnlyMemory<byte>(byteData);

// Execute the query
var contents = new List<AIContent>
{
    new TextContent("What does Evans mean with Granularity?"),
    new DataContent(readOnlyData, "application/pdf")
};

var chatMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, contents);

var result = await client.AsAIAgent().RunAsync(chatMessage);

Console.WriteLine(result.Text);

