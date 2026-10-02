using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;

DotNetEnv.Env.Load();
#pragma warning disable OPENAI001
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetResponsesClient();
#pragma warning restore OPENAI001

var routerAgent = client
    .AsAIAgent(
        model: "gpt-5.4",
        instructions: "You are first responder with a role of routing the call to the right service. Route to NegativeAgent when the call .... Route to PositiveAgent when the call ....an analyst with a pessimistic attitude to the issue.",
        name: "RouterAgent"
    );

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

var workflow = AgentWorkflowBuilder
    .CreateHandoffBuilderWith(routerAgent)
    .WithHandoff(routerAgent, [negativeAgent, positiveAgent])
    .WithHandoff(negativeAgent, routerAgent)
    .WithHandoff(positiveAgent, routerAgent)
    .EnableReturnToPrevious()
    .Build();
    
    var agent = workflow.AsAIAgent(
        executionEnvironment: InProcessExecution.Concurrent,
        includeExceptionDetails: true,
        includeWorkflowOutputsInResponse: true
        );