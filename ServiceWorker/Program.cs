using ServiceWorker;
using ServiceWorker.Service;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Register custom services
        services.AddSingleton<BookingService>();
        services.AddSingleton<CsvService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();