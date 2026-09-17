
using System.ClientModel;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Responses;

DotNetEnv.Env.Load();
#pragma warning disable OPENAI001
var agent = new OpenAIClient(new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAIAPI_KEY")))
    .GetResponsesClient()
#pragma warning restore OPENAI001
    .AsAIAgent(
        model: "gpt-6-astra",
        instructions: "You are a thoughtful analyser. Think step by step."
        );
        
AgentRunOptions options = new()
{
    AllowBackgroundResponses = true
};

AgentSession session = await agent.CreateSessionAsync();

await foreach (var update in agent.RunStreamingAsync("Why are large language models dominating the field of deep learning?", session, options))
{
    Console.Write(update.Text);
#pragma warning disable MEAI001
    options.ContinuationToken = update.ContinuationToken;
#pragma warning restore MEAI001
    break;
};

await foreach (var update in agent.RunStreamingAsync(session, options))
{
    Console.Write(update.Text);
};

