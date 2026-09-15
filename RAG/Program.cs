
using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using Microsoft.SemanticKernel.Text;
using Npgsql;
using OpenAI.Embeddings;
using Pgvector;

var client = GetClientInstance();
var embeddings = GetEmbeddings();

EmbeddingClient GetClientInstance()
{
    return new OpenAIClient(
            new ApiKeyCredential("text"),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("http://127.0.0.1:1234/v1")
            })
        .GetEmbeddingClient("text-embedding-nomic-embed-text-v1.5-embedding");
}

async Task<OpenAIEmbeddingCollection> GetEmbeddings() 
{
    string plainText = await File.ReadAllTextAsync("text.txt");
#pragma warning disable SKEXP0050
    var chunkedText = TextChunker.SplitPlainTextParagraphs(
#pragma warning restore SKEXP0050
        lines: plainText.Split('\n'),
        maxTokensPerParagraph: 200,
        overlapTokens: 30
    );
    return await client.GenerateEmbeddingsAsync(chunkedText);
}



// foreach (var embedding in embeddings.Value)
// {
//     var subjectNumbers = embedding.ToFloats().ToArray();
//     for (int i = 0; i < subjectNumbers.Length; i++)
//     {
//         Console.Write(subjectNumbers[i] + "_" );
//     }
//
//     Console.WriteLine();
// }


var dataSourceBuilder =
    new NpgsqlDataSourceBuilder("Host=localhost;Username=postgres;Password=postgres;Database=pgvector");
dataSourceBuilder.UseVector();
await using var dataSource = dataSourceBuilder.Build();

var connection = await dataSource.OpenConnectionAsync();

await using (var cmd = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS vector", connection))
{
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = new NpgsqlCommand(
                 "CREATE TABLE IF NOT EXISTS documents (id serial PRIMARY KEY, embedding vector(768))", connection))
{
    await cmd.ExecuteNonQueryAsync();
}

// foreach (var embedding in embeddings.Value)
// {
//     await using (var cmd = new NpgsqlCommand("INSERT INTO documents (embedding) VALUES ($1)", connection))
//     {
//         cmd.Parameters.AddWithValue(new Vector(embedding.ToFloats()));
//         await cmd.ExecuteNonQueryAsync();
//     }
// }
