namespace ServiceWorker;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    // <indsæt noget RabbitMQ connectionfactory kode her!>
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started");
        // 1. <indsæt noget RabbitMQ Query + serialiserings kode her!>
        // 2. <Tilføj BookingDTO fra køen til lokal Repository-klasse!>
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("Worker stopped");
    }
}