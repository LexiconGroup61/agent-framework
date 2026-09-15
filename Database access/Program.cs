using System.ComponentModel;
using Database_access;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;
using OpenAI.Chat;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;


DotNetEnv.Env.Load();
#pragma warning disable OPENAI001
var agent = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetResponsesClient()
#pragma warning restore OPENAI001
    .AsAIAgent(
        model: "gpt-5.4",
        instructions: "You are a company assistant tasked with proposing a plan for a company party.",
        tools: [new ApprovalRequiredAIFunction(AIFunctionFactory.Create(GetPeople))]
    );

AgentSession session = await agent.CreateSessionAsync();
var response = await agent.RunAsync("Make a plan for our party on October 16. Include all persons between the age 34 and 52 in an organization committee.", session);

var approval = response.Messages
    .SelectMany(x => x.Contents)
    .OfType<ToolApprovalRequestContent>()
    .ToList();

if (approval.Count > 0)
{
    ToolApprovalRequestContent requestContent = approval.First();

    var toolCall = (FunctionCallContent) requestContent.ToolCall;


    Console.WriteLine("Allow the agent to execute " + toolCall.Name + "? Y/N");
    string userInput = Console.ReadLine();

    bool userInputValid = userInput.ToLower() == "y";

    var newMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(userInputValid)]);

    var responseB = await agent.RunAsync(newMessage, session);

    Console.WriteLine(responseB.Text);
}
else
{
    Console.WriteLine(response.Text);
}



[Description("Get persons within the age range specified")]
List<Person> GetPeople(
    [Description("The lowest age from which the range will be taken")] int fromAge,
    [Description("The highest age to which the range will be taken")] int toAge
    )
{
    var db = new AgentDbContext();
    db.Database.EnsureCreated();

    return db.Persons.Where(p => p.Age >= fromAge && p.Age <= toAge).ToList();
}

