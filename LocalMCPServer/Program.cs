var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();