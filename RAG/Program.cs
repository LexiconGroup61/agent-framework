
using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Data;
using OpenAI;
using Microsoft.SemanticKernel.Text;
using Npgsql;
using OpenAI.Embeddings;
using OpenAI.Responses;
using Pgvector;
using RAG;
using TextSearchProvider = Microsoft.Agents.AI.TextSearchProvider;
using TextSearchProviderOptions = Microsoft.Agents.AI.TextSearchProviderOptions;

TextSearchProviderOptions options = new TextSearchProviderOptions()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
    RecentMessageMemoryLimit = 6
};

async Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchFunction(string message, CancellationToken token)
{
    List<TextSearchProvider.TextSearchResult> search = new();
    var item = new TextSearchProvider.TextSearchResult()
        {
            Text = "This is a test",
            SourceName = "Test",
            SourceLink = "http://www.saab.com",
            RawRepresentation = "test again"
        };
    search.Add(item);
    return search;
}


DotNetEnv.Env.Load();

ChatClientAgent client = new OpenAIClient(new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAIAPI_KEY")))
    .GetResponsesClient()
    .AsAIAgent(new ChatClientAgentOptions()
        {
            AIContextProviders = [new TextSearchProvider(SearchFunction, options)],
            ChatOptions = new()
            {
                ModelId = "gpt-6-astra",
                Instructions = "Use the provided context in your answers and cite the source documents."
            }
        }
        
    
    );

var session = client.CreateSessionAsync();
var response = client.RunAsync("");



