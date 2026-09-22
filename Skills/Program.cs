using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Compaction;
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
#pragma warning disable MAAI001
        AIContextProviders = [skillsProvider, new CompactionProvider(
            new SummarizationCompactionStrategy(
                chatClient: new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY")).GetChatClient("gpt-4").AsIChatClient(),
                summarizationPrompt: "Summarize the main points, but keep numerical data in all cases",
                trigger: CompactionTriggers.TokensExceed(30000),
                minimumPreservedGroups: 7)
            )]
#pragma warning restore MAAI001
    });
    
var response = agent.RunAsync("What rate of VAT should Good-Corp add for the aerobics class sold?");

Console.WriteLine(response.Result.Text);