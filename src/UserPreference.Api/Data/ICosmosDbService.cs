namespace UserPreference.Api.Data;

public interface ICosmosDbService
{
    Task<T?> GetItemAsync<T>(string id) where T : class;
    Task<T> CreateItemAsync<T>(T item) where T : class;
    Task<T> UpdateItemAsync<T>(T item) where T : class;
    Task<bool> DeleteItemAsync<T>(string id) where T : class;
    Task<IEnumerable<T>> QueryItemsAsync<T>(string query) where T : class;
}
