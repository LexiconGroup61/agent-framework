using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;

var skillsProvider = new AgentSkillsProvider(
    skillPath: Path.Combine(AppContext.BaseDirectory, "skills")
);

DotNetEnv.Env.Load();
var agent = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetChatClient("gpt-6-astra")
    .AsAIAgent(new ChatClientAgentOptions()
    {
        ChatOptions = new() { Instructions = "You are a helpful assistant for Good-Corp, a commercial business based in Sweden." },
        AIContextProviders = [skillsProvider]
    });
    
var response = agent.RunAsync("What rate of VAT should Good-Corp add for the aerobics class sold?");

Console.WriteLine(response.Result.Text);