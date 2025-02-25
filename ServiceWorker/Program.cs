using ServiceWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ServiceWorker.Worker>();

var host = builder.Build();
host.Run();
