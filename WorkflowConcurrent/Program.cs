using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;

DotNetEnv.Env.Load();
#pragma warning disable OPENAI001
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetResponsesClient();
#pragma warning restore OPENAI001

var negativeAgent = client
    .AsAIAgent(
        model: "gpt-5.4",
        instructions: "You are an analyst with a pessimistic attitude to the issue.",
        name: "NegativeAgent"
    );
    
var positiveAgent = client
    .AsAIAgent(
        model: "gpt-5.4",
        instructions: "You are an analyst with an optimistic attitude to the issue.",
        name: "PositiveAgent"
    );

var workflow = AgentWorkflowBuilder.BuildConcurrent([negativeAgent, positiveAgent], BalancedView);

List<ChatMessage> BalancedView(IList<List<ChatMessage>> responses)
{
    var negative = responses[0][1].Text;
    var positive = responses[1][1].Text;
    string balanced = $"Negative view: {negative} \nPositive view: {positive}";
    return [new ChatMessage(ChatRole.Assistant, balanced)];
}

List<ChatMessage> question = [new ChatMessage(ChatRole.User, "Is now a good time to start a car rental company?")];

Run execution = await InProcessExecution.RunAsync(workflow, question);

foreach (var item in execution.NewEvents)
{
    if(item is WorkflowOutputEvent output && output.Is<List<ChatMessage>> (out var messages))
    {
        Console.WriteLine(messages[0].Contents[0].ToString());
    }
}