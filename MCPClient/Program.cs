

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Compaction;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;
using OpenAI.Chat;

await using var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new StdioClientTransportOptions()
        {
            Name = "McpServerTrial",
            Command = "npx",
            Arguments = ["-y", "@notionhq/notion-mcp-server"]
        }));

foreach (var item  in await mcpClient.ListToolsAsync())
{
    Console.WriteLine(item);
}


// var agent = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
//     .GetChatClient("gpt-6-astra")
//     .AsAIAgent(
//         instructions: "You are a helpful assistant for Good-Corp, a commercial business based in Sweden." ,
//         tools: [.. mcpClient.Cast<AITool>()]
//     );
