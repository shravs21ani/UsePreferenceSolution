using Microsoft.Azure.ServiceBus;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace UserPreference.Api.Data;

public class ServiceBusService : IServiceBusService, IDisposable
{
    private readonly ITopicClient _topicClient;
    private readonly ILogger<ServiceBusService> _logger;

    public ServiceBusService(string connectionString, string topicName, ILogger<ServiceBusService> logger)
    {
        _topicClient = new TopicClient(connectionString, topicName);
        _logger = logger;
    }

    public async Task PublishMessageAsync<T>(string eventType, T message)
    {
        await PublishMessageAsync(eventType, message as object);
    }

    public async Task PublishMessageAsync(string eventType, object message)
    {
        try
        {
            var messageBody = JsonSerializer.Serialize(message);
            var serviceBusMessage = new Message(System.Text.Encoding.UTF8.GetBytes(messageBody))
            {
                Label = eventType,
                ContentType = "application/json"
            };

            // Add custom properties
            serviceBusMessage.UserProperties["EventType"] = eventType;
            serviceBusMessage.UserProperties["Timestamp"] = DateTime.UtcNow.ToString("O");
            serviceBusMessage.UserProperties["Source"] = "UserPreference.Api";

            await _topicClient.SendAsync(serviceBusMessage);
            
            _logger.LogInformation("Published message of type {EventType} to Service Bus", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message of type {EventType} to Service Bus", eventType);
            throw;
        }
    }

    public void Dispose()
    {
        _topicClient?.CloseAsync();
    }
}
