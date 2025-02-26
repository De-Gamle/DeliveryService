using ServiceWorker.Models;

namespace ServiceWorker.Service
{
    public class CsvService
    {
        private readonly string _outputDirectory;
        private readonly ILogger<CsvService> _logger;

        public CsvService(IConfiguration configuration, ILogger<CsvService> logger)
        {
            _outputDirectory = configuration["CsvOutputDirectory"] ?? "/app/data";
            _logger = logger;
            
            // Ensure the directory exists
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
                _logger.LogInformation($"Created output directory: {_outputDirectory}");
            }
        }

        public async Task WriteShipmentToCsvAsync(ShippingRequest request)
        {
            try
            {
                var fileName = $"shipment_{DateTime.Now:yyyyMMdd}.csv";
                var filePath = Path.Combine(_outputDirectory, fileName);
                var fileExists = File.Exists(filePath);

                using var writer = new StreamWriter(filePath, append: true);
                
                // Write header if file is new
                if (!fileExists)
                {
                    await writer.WriteLineAsync(ShippingRequest.CsvHeader);
                    _logger.LogInformation($"Created new CSV file: {filePath}");
                }
                
                // Write data
                await writer.WriteLineAsync(request.ToCsvString());
                _logger.LogInformation($"Added shipment with ID {request.Id} to {fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error writing shipment to CSV: {ex.Message}");
                throw;
            }
        }
    }
}