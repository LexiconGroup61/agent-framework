
using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using Microsoft.SemanticKernel.Text;

var client = new OpenAIClient(
    new ApiKeyCredential("text"),
    new OpenAIClientOptions
    {
        Endpoint = new Uri("http://127.0.0.1:1234/v1")
    })
    .GetEmbeddingClient("text-embedding-nomic-embed-text-v1.5-embedding");
    

string plainText = await File.ReadAllTextAsync("text.txt");
#pragma warning disable SKEXP0050
var chunkedText = TextChunker.SplitPlainTextParagraphs(
#pragma warning restore SKEXP0050
    lines: plainText.Split('\n'),
        maxTokensPerParagraph: 200,
        overlapTokens: 30
    );

var embeddings = await client.GenerateEmbeddingsAsync(chunkedText);

foreach (var embedding in embeddings.Value)
{
    var subjectNumbers = embedding.ToFloats().ToArray();
    for (int i = 0; i < subjectNumbers.Length; i++)
    {
        Console.Write(subjectNumbers[i] + "_" );
    }

    Console.WriteLine();
}


