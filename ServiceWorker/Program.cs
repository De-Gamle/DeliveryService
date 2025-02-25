using DeliveryService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Tilføj Serilog konfiguration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/delivery-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Tilføj controllers
builder.Services.AddControllers();

// Tilføj Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrer repository som singleton for at dele data mellem worker og controller
builder.Services.AddSingleton<IDeliveryRepository, DeliveryRepository>();

// Registrer worker-klassen
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Konfigurer HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();