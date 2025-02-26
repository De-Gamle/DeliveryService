namespace ServiceWorker.Models
{
    public class ShippingRequest
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public double Weight { get; set; }
        
        // For CSV format
        public string ToCsvString()
        {
            return $"{Id},{CustomerName},{DeliveryAddress},{PostalCode},{City},{RequestDate:yyyy-MM-dd HH:mm:ss},{DeliveryDate:yyyy-MM-dd HH:mm:ss},{PackageType},{Weight}";
        }
        
        // CSV header
        public static string CsvHeader => "Id,CustomerName,DeliveryAddress,PostalCode,City,RequestDate,DeliveryDate,PackageType,Weight";
    }
}