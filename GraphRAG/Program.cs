


using Neo4j.AgentFramework.GraphRAG;
using Neo4j.Driver;

var driver = GraphDatabase.Driver(new Uri("localhost"), AuthTokens.Basic("user", "password"));

var neo4jProvider = new Neo4jContextProvider(
    driver,
    new Neo4jContextProviderOptions(
    {
        IndexName = "",
        IndexType = IndexType.Hybrid,
        RetrievalQuery = "",
        ContextPrompt = ""
        
    }
        )
    );
    