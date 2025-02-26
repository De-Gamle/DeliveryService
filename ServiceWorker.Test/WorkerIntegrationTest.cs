using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using ServiceWorker.Models;
using ServiceWorker.Service;
using System.IO;
using System.Text;
using System.Text.Json;
using ServiceWorker;
using Xunit;

public class WorkerIntegrationTests
{
    private readonly Mock<ILogger<Worker>> _mockLogger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "shipping_requests";

    public WorkerIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<Worker>>();

        // Opret RabbitMQ-forbindelsen (brug en eksisterende RabbitMQ-tjeneste eller lokal)
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    [Fact]
    public void Worker_ProcessesMessageFromUploadedFile()
    {
        // Arrange: Læs filen med data
        var filePath = "C:/Users/rasmu/Downloads/shipment_data.json";
        var fileData = File.ReadAllText(filePath);

        // Deserialiser JSON-dataen til et ShippingRequest-objekt
        var shippingRequest = JsonSerializer.Deserialize<ShippingRequest>(fileData);

        if (shippingRequest != null)
        {
            // Send besked til RabbitMQ
            var message = JsonSerializer.Serialize(shippingRequest);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
        }

        // Verificer, at beskeden er blevet sendt til køen (kun test på, at RabbitMQ kommunikerer)
        var result = _channel.MessageCount(QueueName);
        Xunit.Assert.True(result > 0, "Message was not sent to the queue");

        // Eventuelt: Verificer, om arbejderen behandler beskeden korrekt (f.eks. ved at kontrollere logs eller CSV-fil)
    }

    // Cleanup
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
