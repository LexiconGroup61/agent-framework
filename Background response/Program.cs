

using System.ClientModel;
using System.Globalization;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Responses;

DotNetEnv.Env.Load();
#pragma warning disable OPENAI001
var agent = new OpenAIClient(new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAIAPI_KEY")))
    .GetResponsesClient()
#pragma warning disable OPENAI001
    .AsAIAgent(
        model: "gpt-6-astra",
        instructions: "You are a thoughtful analyser. Think step by step."
    );

AgentRunOptions options = new()
{
    AllowBackgroundResponses = true
};

AgentSession session = await agent.CreateSessionAsync();

var response = await agent.RunAsync("Why is large language models dominating the field of deep learning?", session, options);

#pragma warning disable MEAI001
while (response.ContinuationToken is not null)
{
    Console.WriteLine(response.Text);
    options.ContinuationToken = response.ContinuationToken;
    response = await agent.RunAsync(session, options);
}
#pragma warning restore MEAI001

Console.WriteLine(response.Text);