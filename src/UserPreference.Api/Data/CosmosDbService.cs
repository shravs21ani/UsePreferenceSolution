using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace UserPreference.Api.Data;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;
    private readonly ILogger<CosmosDbService> _logger;

    public CosmosDbService(string connectionString, string databaseName, string containerName)
    {
        var client = new CosmosClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _container = database.GetContainer(containerName);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CosmosDbService>();
    }

    public async Task<T?> GetItemAsync<T>(string id) where T : class
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item with ID {Id} from Cosmos DB", id);
            throw;
        }
    }

    public async Task<T> CreateItemAsync<T>(T item) where T : class
    {
        try
        {
            // Use reflection to get the ID property
            var idProperty = typeof(T).GetProperty("Id");
            var id = idProperty?.GetValue(item)?.ToString();
            
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Item must have an Id property");
            }

            var response = await _container.CreateItemAsync(item, new PartitionKey(id));
            return response.Resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item in Cosmos DB");
            throw;
        }
    }

    public async Task<T> UpdateItemAsync<T>(T item) where T : class
    {
        try
        {
            // Use reflection to get the ID property
            var idProperty = typeof(T).GetProperty("Id");
            var id = idProperty?.GetValue(item)?.ToString();
            
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Item must have an Id property");
            }

            var response = await _container.UpsertItemAsync(item, new PartitionKey(id));
            return response.Resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item in Cosmos DB");
            throw;
        }
    }

    public async Task<bool> DeleteItemAsync<T>(string id) where T : class
    {
        try
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with ID {Id} from Cosmos DB", id);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryItemsAsync<T>(string query) where T : class
    {
        try
        {
            var queryDefinition = new QueryDefinition(query);
            var iterator = _container.GetItemQueryIterator<T>(queryDefinition);
            var results = new List<T>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying items from Cosmos DB with query: {Query}", query);
            throw;
        }
    }
}
