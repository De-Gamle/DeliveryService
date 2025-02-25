using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace DeliveryService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDeliveryRepository _deliveryRepository;
    private IConnection _connection;
    private IModel _channel;
    private const string QueueName = "parcels";

    public Worker(ILogger<Worker> logger, IDeliveryRepository deliveryRepository)
    {
        _logger = logger;
        _deliveryRepository = deliveryRepository;
        
        // RabbitMQ connectionfactory kode
        var factory = new ConnectionFactory
        {
            HostName = "localhost", // Erstat med din RabbitMQ server
            UserName = "guest",     // Erstat med dine credentials
            Password = "guest"      // Erstat med dine credentials
        };
        
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
            _logger.LogInformation("RabbitMQ forbindelse oprettet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved oprettelse af RabbitMQ forbindelse");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started");

        // 1. RabbitMQ Query + serialiserings kode
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Modtaget besked: {Message}", message);
                
                // Deserialiser til ParcelDeliveryDTO
                var delivery = JsonSerializer.Deserialize<ParcelDeliveryDTO>(message);
                
                if (delivery != null)
                {
                    // 2. Tilføj ParcelDeliveryDTO fra køen til lokal Repository-klasse
                    _deliveryRepository.Put(delivery);
                    _logger.LogInformation("Levering tilføjet til repository: {Id}", delivery.Id);
                }
                
                // Bekræft at beskeden er behandlet
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejl ved behandling af besked");
                // Nack beskeden, så den sendes tilbage til køen
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };
        
        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false, // Manuel bekræftelse af beskeder
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        
        _logger.LogInformation("Worker stopped");
        
        // Oprydning
        _channel?.Close();
        _connection?.Close();
    }
}