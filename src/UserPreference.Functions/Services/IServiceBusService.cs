namespace UserPreference.Functions.Services;

public interface IServiceBusService
{
    Task PublishMessageAsync<T>(string eventType, T message);
    Task PublishMessageAsync(string eventType, object message);
}
