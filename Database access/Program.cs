using System.ComponentModel;
using Database_access;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;
using OpenAI.Chat;

DotNetEnv.Env.Load();
#pragma warning disable OPENAI001
var agent = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAIAPI_KEY"))
    .GetResponsesClient()
#pragma warning restore OPENAI001
    .AsAIAgent(
        model: "gpt-5.4",
        instructions: "You are a company assistant tasked with proposing a plan for a company party.",
        tools: [AIFunctionFactory.Create(GetPeople)]
    );

var response = agent.RunAsync("Make a plan for our party on October 16. Include all persons between the age 34 and 52 in an organization committee.");
Console.WriteLine(response.Result.Text);
foreach (var message in response.Result.Messages)
{
    Console.WriteLine(message.Role + ": " + message.Text);
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