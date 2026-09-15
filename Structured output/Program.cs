

using System.ClientModel;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using Structured_output;
using ChatMessage = OpenAI.Chat.ChatMessage;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;

var client = new OpenAIClient(
        new ApiKeyCredential("text"),
        new OpenAIClientOptions
        {
            Endpoint = new Uri("http://127.0.0.1:1234/v1")
        })
    .GetChatClient("gemma-3-4b-it-qat")
    .AsAIAgent();
    
// AgentResponse response = await client.RunAsync<List<Person>>("Generate personal data according to Person schema. Generate data for 10 persons.");

AgentRunOptions runOptions = new AgentRunOptions()
{
    ResponseFormat = ChatResponseFormat.ForJsonSchema<PersonListWrapper>()
};

AgentResponse responseB =
    await client.RunAsync("Generate personal data according to Person schema. Generate data for 10 persons as a list.",
        options: runOptions);

var persons = JsonSerializer.Deserialize<PersonListWrapper>(responseB.Text, JsonSerializerOptions.Web);

foreach (var person in persons.Persons)
{
    Console.WriteLine("Id: " + person.Id);
    Console.WriteLine("First name: " + person.FirstName);
    Console.WriteLine("Last name: " + person.LastName);
    Console.WriteLine("Age : " + person.Age);
    Console.WriteLine("Profession: " + person.Profession);
    Console.WriteLine();
}
