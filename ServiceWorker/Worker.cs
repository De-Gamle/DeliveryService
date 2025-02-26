using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceWorker.Models;

using ServiceWorker.Service;

namespace ServiceWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly BookingService _bookingService;
        private readonly CsvService _csvService;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IModel? _channel;
        private const string QueueName = "shipping_requests";

        public Worker(
            ILogger<Worker> logger, 
            BookingService bookingService, 
            CsvService csvService,
            IConfiguration configuration)
        {
            _logger = logger;
            _bookingService = bookingService;
            _csvService = csvService;
            _configuration = configuration;
            
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            try 
            {
                var factory = new ConnectionFactory 
                { 
                    HostName = _configuration["RabbitMQ:HostName"] ?? "rabbitmq",
                    UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest"
                };
                
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                
                _logger.LogInformation("RabbitMQ connection initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing RabbitMQ connection");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            if (_channel == null)
            {
                _logger.LogError("RabbitMQ channel is not initialized");
                return;
            }

            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    _logger.LogInformation("Received message: {message}", message);
                    
                    var shippingRequest = JsonSerializer.Deserialize<ShippingRequest>(message);
                    
                    if (shippingRequest != null)
                    {
                        // Add to repository
                        _bookingService.Put(shippingRequest);
                        
                        // Write to CSV
                        await _csvService.WriteShipmentToCsvAsync(shippingRequest);
                        
                        // Print sorted list to console (for debugging)
                        var sortedBookings = _bookingService.GetAll();
                        _logger.LogInformation("Current bookings (sorted):");
                        foreach (var booking in sortedBookings)
                        {
                            _logger.LogInformation($"- {booking.Id}: {booking.CustomerName}, Due: {booking.DeliveryDate}");
                        }
                        
                        // Acknowledge message
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel?.BasicNack(ea.DeliveryTag, false, true); // Requeue the message
                }
            };
            
            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                // Task.Delay is used to prevent the thread from being continuously active,
                // which would consume CPU resources unnecessarily. It gives the system a
                // chance to process other tasks and conserve resources.
            }
            
            _logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _channel?.Close();
            _connection?.Close();
            await base.StopAsync(stoppingToken);
        }
    }
}