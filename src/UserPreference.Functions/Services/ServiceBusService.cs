using Microsoft.Azure.ServiceBus;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace UserPreference.Functions.Services;

public class ServiceBusService : IServiceBusService
{
    private readonly ITopicClient _topicClient;
    private readonly ILogger<ServiceBusService> _logger;

    public ServiceBusService(ILogger<ServiceBusService> logger)
    {
        var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
        var topicName = "user-preferences";
        
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
            serviceBusMessage.UserProperties["Source"] = "UserPreference.Functions";

            await _topicClient.SendAsync(serviceBusMessage);
            
            _logger.LogInformation("Published message of type {EventType} to Service Bus", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message of type {EventType} to Service Bus", eventType);
            throw;
        }
    }
}
