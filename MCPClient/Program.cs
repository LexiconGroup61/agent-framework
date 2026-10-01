

using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Compaction;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;

DotNetEnv.Env.Load();

await using var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new StdioClientTransportOptions()
        {
            Name = "McpServerTrial",
            Command = "npx",
            Arguments = ["-y", "@notionhq/notion-mcp-server"],
            EnvironmentVariables = new Dictionary<string, string?>
            {
                ["OPENAPI_MCP_HEADERS"] = JsonSerializer.Serialize (new Dictionary<string, string>
                    {
                        ["Notion-Version"] = "2025-09-03",   
                        ["Authorization"] = $"Bearer {Environment.GetEnvironmentVariable("NOTION_TOKEN")}" 
                    })
            }
        }));

var mcpTools = await mcpClient.ListToolsAsync();

foreach (var item  in mcpTools)
{
    Console.WriteLine(item.Name);
}

// var user = await mcpClient.CallToolAsync("API-get-user");

// Console.WriteLine(user.Content[0]);


#pragma warning disable OPENAI001
var agent = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetResponsesClient()
#pragma warning restore OPENAI001
    .AsAIAgent(
        model: "gpt-6-astra",
        instructions: "You can assist the user with maintaining their Notion Kanban boards." ,
        tools: [.. mcpTools.Cast<AITool>()]
    );

var response = await agent.RunAsync("Add fields for customer name, feedback type, priority, and status to the customer feedback database.");

Console.WriteLine(response.Text);