using System.ClientModel;
using Microsoft.SemanticKernel.Text;
using Npgsql;
using OpenAI;
using OpenAI.Embeddings;

namespace RAG;

public class Vectorization
{
    public EmbeddingClient Client { get; private set; }
    public Task<OpenAIEmbeddingCollection> Embeddings { get; set; }

    public Vectorization()
    {
        Client = new OpenAIClient(
                new ApiKeyCredential("text"),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri("http://127.0.0.1:1234/v1")
                })
            .GetEmbeddingClient("text-embedding-nomic-embed-text-v1.5-embedding");
        
        Embeddings = GetEmbeddings();
    }


    public async void RunEmbedding()
    {

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
                         "CREATE TABLE IF NOT EXISTS documents (id serial PRIMARY KEY, embedding vector(768))",
                         connection))
        {
            await cmd.ExecuteNonQueryAsync();
        }
    }
    
    private string sqlString = """
                       SELECT content, 1 - (embedding <=> $1) AS similarity
                       FROM connection.Vectors 
                       ORDER BY embedding <=> $1
                       LIMIT 5
                       """;
    
    public async Task<OpenAIEmbeddingCollection> GetEmbeddings() 
    {
        string plainText = await File.ReadAllTextAsync("text.txt");
#pragma warning disable SKEXP0050
        var chunkedText = TextChunker.SplitPlainTextParagraphs(
            lines: plainText.Split('\n'),
            maxTokensPerParagraph: 200,
            overlapTokens: 30
        );
#pragma warning restore SKEXP0050
        return await Client.GenerateEmbeddingsAsync(chunkedText);
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


// foreach (var embedding in embeddings.Value)
// {
//     await using (var cmd = new NpgsqlCommand("INSERT INTO documents (embedding) VALUES ($1)", connection))
//     {
//         cmd.Parameters.AddWithValue(new Vector(embedding.ToFloats()));
//         await cmd.ExecuteNonQueryAsync();
//     }
// }

}