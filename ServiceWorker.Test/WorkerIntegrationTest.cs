using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using ServiceWorker.Models;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using ServiceWorker;
using Xunit;

public class WorkerIntegrationTests : IDisposable
{
    private readonly Mock<ILogger<Worker>> _mockLogger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "shipping_requests";

    public WorkerIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<Worker>>();

        try 
        {
            Console.WriteLine("Forsøger at oprette forbindelse til RabbitMQ...");
            
            var factory = new ConnectionFactory() 
            { 
                HostName = "host.docker.internal", 
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            
            _connection = factory.CreateConnection();
            Console.WriteLine("Forbindelse oprettet!");
            
            _channel = _connection.CreateModel();
            Console.WriteLine("Kanal oprettet!");
            
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            Console.WriteLine($"Kø {QueueName} oprettet eller åbnet!");
            
            _channel.QueuePurge(QueueName);
            Console.WriteLine($"Kø {QueueName} renset!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fejl ved tilslutning til RabbitMQ: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    [Fact]
    public void Worker_ProcessesMessageFromUploadedFile()
    {
        try
        {
            var filePath = "C:/Users/rasmu/Downloads/shipment_data.json";
            Xunit.Assert.True(File.Exists(filePath), $"Fil findes ikke: {filePath}");
            
            var fileData = File.ReadAllText(filePath);
            Xunit.Assert.False(string.IsNullOrEmpty(fileData), "Filindhold er tomt");
            
            var shippingRequest = JsonSerializer.Deserialize<ShippingRequest>(fileData);
            Xunit.Assert.NotNull(shippingRequest);
            
            Console.WriteLine("ShippingRequest-objekt deserialiseret korrekt");

            var message = JsonSerializer.Serialize(shippingRequest);
            var body = Encoding.UTF8.GetBytes(message);
            
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            Console.WriteLine($"Sender besked: {message}");
            _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: properties, body: body);
            Console.WriteLine("Besked sendt til RabbitMQ");
            
            Thread.Sleep(5000);
            
            var result = _channel.MessageCount(QueueName);
            Console.WriteLine($"Antal beskeder i kø '{QueueName}': {result}");
            Xunit.Assert.True(result > 0, $"Beskeden blev ikke sendt til køen. Antal er {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fejl under test: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _connection?.Close();
            Console.WriteLine("RabbitMQ forbindelser lukket");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fejl ved oprydning: {ex.Message}");
        }
    }
}