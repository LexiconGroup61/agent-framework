using System.Text.Json;
using ModelContextProtocol.Client;

await using var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new StdioClientTransportOptions()
{
    Name = "McpServerRegistry",
    Command = "dotnet",
    Arguments = ["run", "--project", "/Users/lexvax-larare3/Documents/Lexicon coding/agent-framework/LocalMCPServer/LocalMCPServer.csproj"],
}));

var mcpTools = await mcpClient.ListToolsAsync();

foreach (var item  in mcpTools)
{
    Console.WriteLine(item.Name);
}